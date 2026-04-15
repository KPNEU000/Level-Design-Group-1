using UnityEditor.Animations;
using UnityEngine;
using System.Collections;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true;

    [Header("Side")]
    public DialogueSide mySide;

    float baseIllumination = 4f;
    float flashIllumination = 10f;
    float flashDuration = 1f;


    private Animator anim;
    private Renderer[] rends;
    private MaterialPropertyBlock propBlock;

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
        GameManager.Ins.AllPiecesCollected += CeaseFire;
        SetIllumination(baseIllumination);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot("MEDIUM");
        }
    }

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
                    Shoot("MEDIUM");
                }
                break;

            case DialogueType.High:
                // everyone shoots
                Shoot("HIGH");
                break;
        }
    }

    void OnDialogueEnd(DialogueType type, DialogueSide incomingSide)
    {
        // only react to your own side
        if (incomingSide != mySide) return;

        //might be useful
        //EndFlash();

        SetAnim("IsAimIdle");
    }

    void Shoot(string level)
    {
        Debug.Log(name + " fires on " + level + " dialogue!");
        SetAnim("IsFire");


        // placeholder for stuff like anims basically feel free to be creative here ig

    }

    void CeaseFire()
    {
        Debug.Log("got here soldier");
        SetAnim("IsAimToDown");
    }


    //ANIMATION
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

    void EndStandShoot()
    {
        SetAnim("IsAimIdle");
    }

    //FLASHING
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
}