using UnityEngine;
using System.Collections;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true;
    public int damage;

    [Header("Side")]
    public DialogueSide mySide;

    [SerializeField] float startYRotation = 0f;
    [SerializeField] float shootRange = 30f;

    [Header("Bias Settings")]
    public Vector3 biasBaselines = new Vector3(0.05f, 0.40f, 0.70f);
    public Vector3 biasCeilings = new Vector3(0.2f, 0.75f, 1.00f);
    public float biasLerpSpeed = 8f;
    public float biasDecaySpeed = 3f;

    float _targetBias;
    float _currentBias;
    bool _dialogueActive;
    DialogueType _activeType;

    float baseIllumination = 4f;
    float flashIllumination = 10f;
    float flashDuration = 1f;

    private Animator anim;
    private Renderer[] rends;
    private MaterialPropertyBlock propBlock;

    public GameObject muzzleFlashEffects;
    public GameObject gun;
    public GameObject bulletShellPrefab;
    public Transform ejectionPoint;

    bool canShoot = true;

    private void OnEnable()
    {
        DialogueManager.OnDialogueTriggered += OnDialogue;
        DialogueManager.OnDialogueEnd += OnDialogueEnd;
        anim = GetComponent<Animator>();
        rends = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
        SetAnim("IsAimIdle");
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueTriggered -= OnDialogue;
        DialogueManager.OnDialogueEnd -= OnDialogueEnd;
        GameManager.Ins.AllPiecesCollected -= CeaseFire;
    }

    void Start()
    {
        damage = isBasic ? 15 : 25;
        GameManager.Ins.AllPiecesCollected += CeaseFire;
        SetIllumination(baseIllumination);
    }

    void Update()
    {
        if (_dialogueActive)
        {
            float liveVolume = mySide == DialogueSide.Son
                ? DialogueManager.SonVolume
                : DialogueManager.DaughterVolume;

            float baseline = BiasBaseline(_activeType);
            float ceiling = BiasCeiling(_activeType);

            _targetBias = Mathf.Lerp(baseline, ceiling, liveVolume);
        }
        else
        {
            _targetBias = 0f;
        }

        _currentBias = Mathf.Lerp(
            _currentBias,
            _targetBias,
            Time.deltaTime * (_dialogueActive ? biasLerpSpeed : biasDecaySpeed)
        );

        SetBias(_currentBias);
    }

    void OnDialogue(DialogueType type, DialogueSide incomingSide)
    {
        if (incomingSide != mySide) return;

        _dialogueActive = true;
        _activeType = type;

        TriggerFlash();


        switch (type)
        {
            case DialogueType.Medium:
                if (isBasic) HandleShoot("MEDIUM");
                break;
            case DialogueType.High:
                HandleShoot("HIGH");
                break;
        }
    }

    void OnDialogueEnd(DialogueType type, DialogueSide incomingSide)
    {
        if (incomingSide != mySide) return;

        _dialogueActive = false;
        SetAnim("IsAimIdle");
    }

    float BiasBaseline(DialogueType type) => type switch
    {
        DialogueType.Low => biasBaselines.x,
        DialogueType.Medium => biasBaselines.y,
        DialogueType.High => biasBaselines.z,
        _ => 0f
    };

    float BiasCeiling(DialogueType type) => type switch
    {
        DialogueType.Low => biasCeilings.x,
        DialogueType.Medium => biasCeilings.y,
        DialogueType.High => biasCeilings.z,
        _ => 0f
    };

    void SetBias(float value)
    {
        propBlock.SetFloat("_FresnelBias", value);
        foreach (Renderer rend in rends)
            rend.SetPropertyBlock(propBlock);
    }

    void HandleShoot(string level)
    {
        float randomDelay = Random.Range(.3f, .5f);
        Invoke("Shoot", randomDelay);
    }

    void Shoot() => SetAnim("IsFire");

    void CeaseFire()
    {
        canShoot = false;
        SetAnim("IsAimToDown");
    }

    void SetAnim(string name)
    {
        anim.SetBool("IsFire", false);
        anim.SetBool("IsAimIdle", false);
        if (name == "IsAimToDown") anim.SetTrigger("IsAimToDown");
        else anim.SetBool(name, true);
    }

    void FireRaycast()
    {
        if (!canShoot) return;

        Vector3 origin = ejectionPoint.position;
        Vector3 direction = ejectionPoint.forward;

        float spread = 0.1f;
        direction.x += Random.Range(-spread, spread);
        direction.z += Random.Range(-spread, spread);
        direction.y = 0f;
        direction.Normalize();

        Debug.DrawRay(origin, direction * shootRange, Color.red, 1f);

        LayerMask combinedMask = LayerMask.GetMask("Blocking", "Player");

        if (Physics.Raycast(origin, direction, out RaycastHit hit, shootRange, combinedMask))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Blocking"))
            {
                Debug.Log($"[{name}] shot blocked by cover");
                Debug.DrawRay(origin, direction * shootRange, Color.blue, 1f);
                return;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log($"[{name}] player hit");
                Debug.DrawRay(origin, direction * shootRange, Color.green, 1f);
                GameManager.Ins.RemoveHealth(damage);
            }
        }
    }

    public void triggerMuzzleEffects()
    {
        muzzleFlashEffects.GetComponent<ParticleSystem>().Stop();
        muzzleFlashEffects.GetComponentInChildren<ParticleSystem>().Stop();
        muzzleFlashEffects.GetComponent<ParticleSystem>().Play();
        muzzleFlashEffects.GetComponentInChildren<ParticleSystem>().Play();

        GameObject bullet = Instantiate(bulletShellPrefab, ejectionPoint.position,
            new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        bullet.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0.5f, 5f), Random.Range(0.5f, 15f), -1));

        FireRaycast();
    }

    public void triggerMuzzleSounds() => gun.GetComponent<AudioSource>().Play();

    void SetIllumination(float value)
    {
        propBlock.SetFloat("_SelfIllumination", value);
        foreach (Renderer rend in rends) rend.SetPropertyBlock(propBlock);
    }

    public void TriggerFlash() => StartCoroutine(FlashRoutine());

    IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            SetIllumination(Mathf.Lerp(baseIllumination, flashIllumination, Mathf.SmoothStep(0f, 1f, elapsed / flashDuration)));
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            SetIllumination(Mathf.Lerp(flashIllumination, baseIllumination, Mathf.SmoothStep(0f, 1f, elapsed / flashDuration)));
            yield return null;
        }
        SetIllumination(baseIllumination);
    }
}

