using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true;

    [Header("Side")]
    public DialogueSide mySide;

    private void OnEnable()
    {
        DialogueManager.OnDialogueTriggered += OnDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueTriggered -= OnDialogue;
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

        // placeholder for stuff like anims basically feel free to be creative here ig

    }
}