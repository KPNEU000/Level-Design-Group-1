using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public AudioSource audioSource;

    public DialogueGroup[] dialogueGroups;

    // event: soldiers listen to this
    public static event Action<DialogueType, DialogueSide> OnDialogueTriggered;

    public void PlayDialogue(DialogueType type, DialogueSide side)
    {
        foreach (var group in dialogueGroups)
        {
            if (group.type == type)
            {
                if (group.clips == null || group.clips.Length == 0)
                {
                    Debug.LogWarning("No clips assigned for " + type);
                    return;
                }

                AudioClip chosenClip = group.clips[UnityEngine.Random.Range(0, group.clips.Length)];

                audioSource.clip = chosenClip;
                audioSource.Play();

                // broadcast to soldiers
                OnDialogueTriggered?.Invoke(type, side);

                return;
            }
        }

        Debug.LogWarning("Dialogue type not found: " + type);
    }
}