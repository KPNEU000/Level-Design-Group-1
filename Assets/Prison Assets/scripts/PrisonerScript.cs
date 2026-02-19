using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PrisonerScript : MonoBehaviour
{
    [SerializeField] private Animator prisonerAnimator;

    private Transform playerTransform;

    public int animType = 0;
    float detectionDistance = 3f;

    private AudioSource audioSource;
    public List<AudioClip> cries = new List<AudioClip>();

    bool wasInRange = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (animType == 0)
        {
            prisonerAnimator.SetBool("IsSitting", true);
            prisonerAnimator.SetBool("IsWaving", false);
            prisonerAnimator.SetBool("IsShakeDoor", false);
            wasInRange = false;
            return;
        }

        if (animType == 1)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool inRange = distance < detectionDistance;

            if (inRange && !wasInRange)
            {
                if (cries.Count > 0)
                {
                    audioSource.PlayOneShot(cries[Random.Range(0, cries.Count)]);
                }
            }

            prisonerAnimator.SetBool("IsWaving", inRange);
            prisonerAnimator.SetBool("IsShakeDoor", !inRange);

            wasInRange = inRange;
        }
    }
}
