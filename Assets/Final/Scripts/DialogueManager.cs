using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public AudioSource audioSource;

    public DialogueGroup[] dialogueGroups;

    // events: soldiers listen to these
    public static event Action OnLowDialogue;
    public static event Action OnHighDialogue;

    public void PlayDialogue(DialogueType type)
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

                // trigger events
                if (type == DialogueType.Low)
                {
                    OnLowDialogue?.Invoke();
                }
                else if (type == DialogueType.High)
                {
                    OnHighDialogue?.Invoke();
                }

                return;
            }
        }

        Debug.LogWarning("Dialogue type not found: " + type);
    }
}