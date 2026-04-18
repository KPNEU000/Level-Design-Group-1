using UnityEditor.Animations;
using UnityEngine;
using System.Collections;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true;
    public int damage;

    [Header("Side")]
    public DialogueSide mySide;

    float baseIllumination = 4f;
    float flashIllumination = 10f;
    float flashDuration = 1f;


    private Animator anim;
    private Renderer[] rends;
    private MaterialPropertyBlock propBlock;

    public GameObject muzzleFlashEffects;

    private void OnEnable()
    {
        DialogueManager.OnDialogueTriggered += OnDialogue;
        DialogueManager.OnDialogueEnd += OnDialogueEnd;
        anim = GetComponent<Animator>();
        SetAnim("IsAimIdle");
        rends = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueTriggered -= OnDialogue;
        DialogueManager.OnDialogueEnd -= OnDialogueEnd;
        GameManager.Ins.AllPiecesCollected -= CeaseFire;
    }

    void Start()
    {
        if(isBasic)
        {
            damage = 20;
        }
        else
        {
            damage = 45;
        }
        GameManager.Ins.AllPiecesCollected += CeaseFire;
        SetIllumination(baseIllumination);
    }

    //On the start of any dialogue spawning, check if it's our team and if I shoot or not
    void OnDialogue(DialogueType type, DialogueSide incomingSide)
    {
        // only react to your own side
        if (incomingSide != mySide) return;

        TriggerFlash();

        switch (type)
        {
            case DialogueType.Low:
                // no shooting, but could trigger visuals later
                Debug.Log(name + " hears LOW dialogue");
                break;

            case DialogueType.Medium:
                // only basic soldiers shoot
                if (isBasic)
                {
                    HandleShoot("MEDIUM");
                }
                break;

            case DialogueType.High:
                // everyone shoots
                HandleShoot("HIGH");
                break;
        }
    }

    //Triggered on any dialogue piece end. Add actual data to end shooting colliders when we get to that
    void OnDialogueEnd(DialogueType type, DialogueSide incomingSide)
    {
        // only react to your own side
        if (incomingSide != mySide) return;

        //might be useful in the future to make the following
        //EndFlash();

        SetAnim("IsAimIdle");  //shooting stops
    }


    //call this to start shooting animation and associated data, but it's on a slight random delay
    void HandleShoot(string level)
    {
        float randomDelay = Random.Range(0f, .3f);
        Invoke("Shoot", randomDelay);
        Debug.Log(name + " fires on " + level + " dialogue!");
    }

    //The helper method that HandleShoot uses
    void Shoot()
    {
        SetAnim("IsFire");
    }


    //put arms down. Done at end of game
    void CeaseFire()
    {
        Debug.Log("got here soldier");
        SetAnim("IsAimToDown");
    }


    //animation setter helper
    void SetAnim(string name)
    {
        anim.SetBool("IsFire", false);
        anim.SetBool("IsAimIdle", false);

        if (name == "IsAimToDown")
        {
            anim.SetTrigger("IsAimToDown");
        }
        else
        {
            Debug.Log("setting" + name + "to true");
            anim.SetBool(name, true);
        }
    }

    //FLASHING STUFF
    public void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        float totalDuration = flashDuration;

        // Ramp up
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalDuration;
            float curvedT = Mathf.SmoothStep(0f, 1f, t);
            SetIllumination(Mathf.Lerp(baseIllumination, flashIllumination, curvedT));
            yield return null;
        }

        elapsed = 0f;

        // Ramp down
        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / totalDuration;
            float curvedT = Mathf.SmoothStep(0f, 1f, t);
            SetIllumination(Mathf.Lerp(flashIllumination, baseIllumination, curvedT));
            yield return null;
        }

        SetIllumination(baseIllumination);
    }

    void SetIllumination(float value)
    {
        propBlock.SetFloat("_SelfIllumination", value);
        foreach (Renderer rend in rends)
        {
            rend.SetPropertyBlock(propBlock);
        }
    }

    public void triggerMuzzleEffects()
    {
        muzzleFlashEffects.GetComponent<ParticleSystem>().Stop();
        muzzleFlashEffects.GetComponentInChildren<ParticleSystem>().Stop();
        muzzleFlashEffects.GetComponent<ParticleSystem>().Play();
        muzzleFlashEffects.GetComponentInChildren<ParticleSystem>().Play();
        //play audio clip
    }
}