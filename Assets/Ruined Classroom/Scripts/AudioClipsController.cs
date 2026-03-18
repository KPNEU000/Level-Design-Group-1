using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioClipsController : MonoBehaviour
{
    public TextMeshProUGUI audioText;
    public int currentLitNumber;
    public GameObject player;
    PlayerController playerController;
    AudioSource clipAudioSource;
    string displayString;
    string color1 = "<color=#5FFF6F>";
    string color2 = "<color=#2D5731>";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLitNumber = 0;
        playerController = player.GetComponent<PlayerController>();
        clipAudioSource = player.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlayNumberSound(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlayNumberSound(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) PlayNumberSound(2);
        HandleLighting();
    }

    void PlayNumberSound(int index)
    {
        currentLitNumber = index + 1;
        clipAudioSource.clip = playerController.numberSounds[index];
        clipAudioSource.Play();
    }

    void HandleLighting()
    {
        // default color for numbers
        string num0 = "<color=#2D5731>1</color>";
        string num1 = "<color=#2D5731>2</color>";
        string num2 = "<color=#2D5731>3</color>";

        // highlight current number
        if (clipAudioSource.isPlaying)
        {
            if (currentLitNumber == 0) num0 = "<color=#5FFF6F>1</color>";
            else if (currentLitNumber == 1) num1 = "<color=#5FFF6F>2</color>";
            else if (currentLitNumber == 2) num2 = "<color=#5FFF6F>3</color>";
        }

        // build the display string
        displayString = $"<color=#5FFF6F>PLAY CLIP:</color> {num0} | {num1} | {num2}";

        // assign to TMP
        audioText.text = displayString;
    }
}
