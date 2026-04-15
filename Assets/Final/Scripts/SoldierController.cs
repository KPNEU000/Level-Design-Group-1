using UnityEditor.Animations;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true;

    [Header("Side")]
    public DialogueSide mySide;

    private Animator anim;

    private void OnEnable()
    {
        DialogueManager.OnDialogueTriggered += OnDialogue;
        anim = GetComponent<Animator>();
        SetAnim("IsStandAimIdle");
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueTriggered -= OnDialogue;
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

    void Shoot(string level)
    {
        Debug.Log(name + " fires on " + level + " dialogue!");
        SetAnim("IsStandFire");


        // placeholder for stuff like anims basically feel free to be creative here ig

    }

    void SetAnim(string name)
    {
        anim.SetBool("IsStandFire", false);
        anim.SetBool("IsStandAimIdle", false);

        anim.SetBool(name, true);
    }

    void EndStandShoot()
    {
        SetAnim("IsStandAimIdle");
    }
}