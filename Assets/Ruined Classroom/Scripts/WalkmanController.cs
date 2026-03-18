using UnityEngine;

public class WalkmanController : MonoBehaviour
{
    public Animator animator;
    Transform hand;
    public Vector3 rotation;
    public Vector3 position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        rotation = new Vector3(-75f, 0, 0);
        position = new Vector3(-0.04f, 0.19f, 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetParent(hand);
        transform.localPosition = position;
        transform.localEulerAngles = rotation;
    }
}
