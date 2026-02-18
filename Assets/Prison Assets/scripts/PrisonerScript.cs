using UnityEngine;

public class PrisonerScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Insert Animator")]
    private Animator prisonerAnimator;

    private Transform playerTransform;

    public int animType = 0;
    float detectionDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (animType == 0)
        {
            prisonerAnimator.SetBool("IsSitting", true);
        }

        if (animType == 1)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance < detectionDistance)
            {
                prisonerAnimator.SetBool("IsWaving", true);
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