using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public AudioSource audioSource;
    public bool autoPlayEnabled = true;
    public DialogueGroup[] dialogueGroups;

    // event: soldiers listen to this
    public static event Action<DialogueType, DialogueSide> OnDialogueTriggered;
    public static event Action<DialogueType, DialogueSide> OnDialogueEnd;


    void Start()
    {
        StartCoroutine(AutoPlayDialogue());
    } 

    IEnumerator AutoPlayDialogue()
    {
        yield return new WaitForSeconds(1f);

        while (autoPlayEnabled)
        {
            // pick a random type and sid
            DialogueType randomType = (DialogueType)UnityEngine.Random.Range(0,3);
            DialogueSide randomSide = (DialogueSide)UnityEngine.Random.Range(0,2);

            // find matching group
            DialogueGroup matchedGroup = null;
            foreach (var group in dialogueGroups)
            {
                if (group.type == randomType && group.side == randomSide)
                {
                    matchedGroup = group;
                    break;
                }
            }

            if (matchedGroup != null && matchedGroup.clips.Length > 0)
            {
                // pick random clip from group
                AudioClip clip = matchedGroup.clips[UnityEngine.Random.Range(0, matchedGroup.clips.Length)];

                // play ts
                audioSource.clip = clip;
                audioSource.Play();

                // fire start event
                OnDialogueTriggered?.Invoke(randomType, randomSide);

                // wiat for clip to finish
                yield return new WaitForSeconds(clip.length);

                // fire end event
                OnDialogueEnd?.Invoke(randomType, randomSide);
            }

            // random pause before next clip
            float delay = UnityEngine.Random.Range(0.1f, 2.0f);
            yield return new WaitForSeconds(delay);
        }
    }

    void Update()
    {
        // V1 Debugging
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayDialogue(DialogueType.Low, DialogueSide.Son, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayDialogue(DialogueType.Medium, DialogueSide.Son, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayDialogue(DialogueType.High, DialogueSide.Son, false);
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            PlayDialogue(DialogueType.Low, DialogueSide.Son, true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            PlayDialogue(DialogueType.Medium, DialogueSide.Son, true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            PlayDialogue(DialogueType.High, DialogueSide.Son, true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayDialogue(DialogueType.Low, DialogueSide.Daughter, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayDialogue(DialogueType.Medium, DialogueSide.Daughter, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayDialogue(DialogueType.High, DialogueSide.Daughter, false);
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            PlayDialogue(DialogueType.Low, DialogueSide.Daughter, true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            PlayDialogue(DialogueType.Medium, DialogueSide.Daughter, true);
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            PlayDialogue(DialogueType.High, DialogueSide.Daughter, true);
        }
    }

    public void PlayDialogue(DialogueType type, DialogueSide side, bool end)
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
                if (end)
                {
                    OnDialogueEnd?.Invoke(type, side);
                }
                else
                {
                    OnDialogueTriggered?.Invoke(type, side);
                }

                return;
            }
        }

        Debug.LogWarning("Dialogue type not found: " + type);
    }
}