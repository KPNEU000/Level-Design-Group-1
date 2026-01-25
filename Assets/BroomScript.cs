using UnityEngine;

public class BroomScript : MonoBehaviour
{

    public GameObject character;
    public GameObject bone;


    Vector3 idleAttackPos = new Vector3(-1.102617f, 1.752648f, -0.3680f);
    Vector3 idleAttackRot = new Vector3(-31.566f, -170.472f, 249.595f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Animator anim = character.GetComponent<Animator>();

        // transform.SetParent(bone.transform);
        // transform.localPosition = idleAttackPos;
        // transform.localRotation = Quaternion.Euler(idleAttackRot.x, idleAttackRot.y, idleAttackRot.z);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
