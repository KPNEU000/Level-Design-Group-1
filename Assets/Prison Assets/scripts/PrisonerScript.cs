using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PrisonerScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Insert Animator")]
    private Animator prisonerAnimator;

    private Transform playerTransform;

    public int animType = 0;
    float detectionDistance = 3f;

    private AudioSource audioSource;
    public List<AudioClip> cries = new List<AudioClip>();
    public bool soundPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animType == 0)
        {
            prisonerAnimator.SetBool("IsSitting", true);
            soundPlayed = false;
        }

        if (animType == 1)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (!soundPlayed) {
            audioSource.PlayOneShot(cries[Random.Range(0, cries.Count - 1)]);
            soundPlayed = true;
            }
            if (distance < detectionDistance)
            {
                prisonerAnimator.SetBool("IsWaving", true);
                soundPlayed = false;
                prisonerAnimator.SetBool("IsShakeDoor", false);
            }
            else
            {
                prisonerAnimator.SetBool("IsShakeDoor", true);
                prisonerAnimator.SetBool("IsWaving", false);
            }
        }
    }
}