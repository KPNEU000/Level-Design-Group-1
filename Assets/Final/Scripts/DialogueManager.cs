using UnityEngine;
using System;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Ins { get; private set; }
    public AudioSource audioSource;
    public AudioSource sonAudioSource;
    public AudioSource daughterAudioSource;
    public bool autoPlayEnabled = true;
    public DialogueGroup[] dialogueGroups;

    public static event Action<DialogueType, DialogueSide> OnDialogueTriggered;
    public static event Action<DialogueType, DialogueSide> OnDialogueEnd;
    public static float SonVolume { get; private set; }
    public static float DaughterVolume { get; private set; }

    private float[] _samples = new float[256];
    public bool firstDialogueHappened = false;
    public bool FirstDialogueHappened => firstDialogueHappened;

    [System.Serializable]
    public struct DialogueStep
    {
        public DialogueType type;
        public DialogueSide side;
        public float delayAfter;
    }

    private DialogueStep[][] presets = new DialogueStep[][]
    {
        new DialogueStep[] {
            new DialogueStep { type = DialogueType.Low,    side = DialogueSide.Daughter, delayAfter = 0.5f },
            new DialogueStep { type = DialogueType.Medium, side = DialogueSide.Daughter, delayAfter = 0.1f },
            new DialogueStep { type = DialogueType.Medium, side = DialogueSide.Son,      delayAfter = 0f   }
        },
        new DialogueStep[] {
            new DialogueStep { type = DialogueType.Low,    side = DialogueSide.Son,      delayAfter = 0.3f },
            new DialogueStep { type = DialogueType.Low,    side = DialogueSide.Daughter, delayAfter = 0.2f },
            new DialogueStep { type = DialogueType.High,   side = DialogueSide.Son,      delayAfter = 0f   }
        },
        new DialogueStep[] {
            new DialogueStep { type = DialogueType.Medium, side = DialogueSide.Son,      delayAfter = 0.4f },
            new DialogueStep { type = DialogueType.Medium, side = DialogueSide.Daughter, delayAfter = 0.1f },
            new DialogueStep { type = DialogueType.High,   side = DialogueSide.Daughter, delayAfter = 0.2f },
            new DialogueStep { type = DialogueType.High,   side = DialogueSide.Son,      delayAfter = 0f   }
        },
        new DialogueStep[] {
            new DialogueStep { type = DialogueType.Low,    side = DialogueSide.Daughter, delayAfter = 0.5f },
            new DialogueStep { type = DialogueType.Low,    side = DialogueSide.Son,      delayAfter = 0f   }
        },
        new DialogueStep[] {
            new DialogueStep { type = DialogueType.High,   side = DialogueSide.Daughter, delayAfter = 0.1f },
            new DialogueStep { type = DialogueType.High,   side = DialogueSide.Son,      delayAfter = 0.3f },
            new DialogueStep { type = DialogueType.Medium, side = DialogueSide.Daughter, delayAfter = 0f   }
        }
    };

    void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Debug.LogError($"Multiple instances of DialogueManager in scene, destroying component on {gameObject.name}");
            Destroy(this);
            return;
        }
        Ins = this;

        GameManager.Ins.OnGameProperStart += WakeUp;
    }

    void OnDisable()
    {
        GameManager.Ins.OnGameProperStart -= WakeUp;
    }

    void WakeUp()
    {
        StartCoroutine(AutoPlayDialogue());
    }

    void Update()
    {
        SonVolume = SampleVolume(sonAudioSource);
        DaughterVolume = SampleVolume(daughterAudioSource);

        // (your existing debug key input block here, unchanged)
    }

    // Returns a [0..1] RMS volume for an AudioSource
    float SampleVolume(AudioSource source)
    {
        if (source == null || !source.isPlaying) return 0f;
        source.GetOutputData(_samples, 0);
        float sum = 0f;
        foreach (float s in _samples) sum += s * s;
        return Mathf.Sqrt(sum / _samples.Length);  // RMS
    }

    AudioSource GetAudioSourceForSide(DialogueSide side)
        => side == DialogueSide.Son ? sonAudioSource : daughterAudioSource;

    AudioClip PlayClipForGroup(DialogueType type, DialogueSide side)
    {
        foreach (var group in dialogueGroups)
        {
            if (group.type == type && group.side == side && group.clips?.Length > 0)
            {
                AudioClip clip = group.clips[UnityEngine.Random.Range(0, group.clips.Length)];
                AudioSource source = GetAudioSourceForSide(side);
                source.clip = clip;
                source.Play();
                return clip;
            }
        }
        return null;
    }

    IEnumerator AutoPlayDialogue()
    {
        yield return new WaitForSeconds(1f);

        while (autoPlayEnabled)
        {
            DialogueStep[] preset = presets[UnityEngine.Random.Range(0, presets.Length)];

            foreach (var step in preset)
            {
                AudioClip clip = PlayClipForGroup(step.type, step.side);
                if (clip != null)
                {
                    OnDialogueTriggered?.Invoke(step.type, step.side);
                    yield return new WaitForSeconds(clip.length);
                    OnDialogueEnd?.Invoke(step.type, step.side);
                    firstDialogueHappened = true;
                }
                if (step.delayAfter > 0f)
                    yield return new WaitForSeconds(step.delayAfter);
            }

            float delay = UnityEngine.Random.Range(0.1f, 2.0f);
            yield return new WaitForSeconds(delay);
        }
    }

    public void PlayDialogue(DialogueType type, DialogueSide side, bool end)
    {
        foreach (var group in dialogueGroups)
        {
            if (group.type == type)
            {
                if (end) OnDialogueEnd?.Invoke(type, side);
                else OnDialogueTriggered?.Invoke(type, side);
                return;
            }
        }
        Debug.LogWarning("Dialogue type not found: " + type);
    }
}