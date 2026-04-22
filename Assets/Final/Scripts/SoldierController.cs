using UnityEngine;
using System.Collections;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true;
    public int damage;

    [Header("Side")]
    public DialogueSide mySide;

    [Header("Bias Settings")]
    public Vector3 biasBaselines = new Vector3(0.05f, 0.40f, 0.70f);
    public Vector3 biasCeilings = new Vector3(0.2f, 0.75f, 1.00f);
    public float biasLerpSpeed = 8f;   // how snappy the per-frame tracking is
    public float biasDecaySpeed = 3f;   // how fast bias fades after dialogue ends

    float _targetBias;        // what we're lerping toward
    float _currentBias;       // actual shader value
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
        damage = isBasic ? 20 : 45;
        GameManager.Ins.AllPiecesCollected += CeaseFire;
        SetIllumination(baseIllumination);
    }

    void Update()
    {
        if (_dialogueActive)
        {
            // Pull live volume for our side only
            float liveVolume = mySide == DialogueSide.Son
                ? DialogueManager.SonVolume
                : DialogueManager.DaughterVolume;

            // Get this tier's baseline and ceiling
            float baseline = BiasBaseline(_activeType);
            float ceiling = BiasCeiling(_activeType);

            // Volume drives us from baseline toward ceiling
            _targetBias = Mathf.Lerp(baseline, ceiling, liveVolume);
        }
        else
        {
            _targetBias = 0f;   // decay back to zero when silent
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
            case DialogueType.Low:
                Debug.Log(name + " hears LOW dialogue");
                break;
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

    // --- Bias helpers ---

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
        float randomDelay = Random.Range(0f, .3f);
        Invoke("Shoot", randomDelay);
        Debug.Log(name + " fires on " + level + " dialogue!");
    }

    void Shoot() => SetAnim("IsFire");

    void CeaseFire()
    {
        Debug.Log("got here soldier");
        SetAnim("IsAimToDown");
    }

    void SetAnim(string name)
    {
        anim.SetBool("IsFire", false);
        anim.SetBool("IsAimIdle", false);
        if (name == "IsAimToDown") anim.SetTrigger("IsAimToDown");
        else anim.SetBool(name, true);
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

    void SetIllumination(float value)
    {
        propBlock.SetFloat("_SelfIllumination", value);
        foreach (Renderer rend in rends) rend.SetPropertyBlock(propBlock);
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
    }

    public void triggerMuzzleSounds() => gun.GetComponent<AudioSource>().Play();
}