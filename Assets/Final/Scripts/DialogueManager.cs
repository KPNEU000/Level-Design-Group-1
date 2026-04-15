using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public AudioSource audioSource;

    public DialogueGroup[] dialogueGroups;

    // event: soldiers listen to this
    public static event Action<DialogueType, DialogueSide> OnDialogueTriggered;

    void Update()
    {
        //Debugging
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("erer");
            PlayDialogue(DialogueType.Low, DialogueSide.Son);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayDialogue(DialogueType.Medium, DialogueSide.Son);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayDialogue(DialogueType.High, DialogueSide.Son);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayDialogue(DialogueType.Low, DialogueSide.Daughter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayDialogue(DialogueType.Medium, DialogueSide.Daughter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayDialogue(DialogueType.High, DialogueSide.Daughter);
        }
    }

    public void PlayDialogue(DialogueType type, DialogueSide side)
    {
        foreach (var group in dialogueGroups)
        {
            if (group.type == type)
            {
                // if (group.clips == null || group.clips.Length == 0)
                // {
                //     Debug.LogWarning("No clips assigned for " + type);
                //     return;
                // }

                // AudioClip chosenClip = group.clips[UnityEngine.Random.Range(0, group.clips.Length)];

                // audioSource.clip = chosenClip;
                // audioSource.Play();

                // broadcast to soldiers
                OnDialogueTriggered?.Invoke(type, side);

                return;
            }
        }

        Debug.LogWarning("Dialogue type not found: " + type);
    }
}