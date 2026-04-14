using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [Header("Anger Settings")]
    public float currentAnger = 0f;
    public float angerThreshold = 5f;

    [Header("Anger Gain Per Dialogue")]
    public float attackAnger = 2f;
    public float tauntAnger = 1.5f;
    public float retreatAnger = 0.5f;
    public float holdAnger = 0.2f;

    private bool isAngry = false;

    private void OnEnable()
    {
        DialogueManager.OnDialogueTriggered += ReactToDialogue;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueTriggered -= ReactToDialogue;
    }

    void ReactToDialogue(DialogueType type)
    {
        if (isAngry) return;

        switch (type)
        {
            case DialogueType.Attack:
                IncreaseAnger(attackAnger);
                break;

            // case DialogueType.Taunt:
            //     IncreaseAnger(tauntAnger);
            //     break;

            case DialogueType.Retreat:
                IncreaseAnger(retreatAnger);
                break;

            case DialogueType.Hold:
                IncreaseAnger(holdAnger);
                break;
        }
    }

    void IncreaseAnger(float amount)
    {
        currentAnger += amount;

        Debug.Log(name + " anger: " + currentAnger);

        if (currentAnger >= angerThreshold)
        {
            BecomeAngry();
        }
    }

    void BecomeAngry()
    {
        isAngry = true;

        Debug.Log(name + " is now ANGRY and attacks!");

        // placeholder behavior
        // later like in w2 we can trigger animations and do state machine stuff ig
    }
}