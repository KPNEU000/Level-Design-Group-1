using UnityEngine;

public class SoldierController : MonoBehaviour
{
    [Header("Soldier Type")]
    public bool isBasic = true; // toggle in inspector

    private void OnEnable()
    {
        DialogueManager.OnLowDialogue += OnLow;
        DialogueManager.OnHighDialogue += OnHigh;
    }

    private void OnDisable()
    {
        DialogueManager.OnLowDialogue -= OnLow;
        DialogueManager.OnHighDialogue -= OnHigh;
    }

    void OnLow()
    {
        // only basic soldiers react to low
        if (isBasic)
        {
            Shoot("LOW");
        }
    }

    void OnHigh()
    {
        // all soldiers react to high
        Shoot("HIGH");
    }

    void Shoot(string level)
    {
        Debug.Log(name + " fires on " + level + " dialogue!");
        
        // placeholder for real shooting logic
        // later: animations, projectiles, etc.
    }
}