using UnityEngine;

public class PrisonerScript : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Insert Animator")]
    private Animator prisonerAnimator;

    public int animType = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (animType == 0)
        {
            prisonerAnimator.SetBool("IsSitting", true);
        }
    }
}