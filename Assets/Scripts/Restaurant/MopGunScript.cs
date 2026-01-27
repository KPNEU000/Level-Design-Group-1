using UnityEngine;

public class MopGunScript : MonoBehaviour
{

    public GameObject character;
    public GameObject bone;


    Vector3 idleAttackPos = new Vector3(-1.126f, 1.763f, -0.431f);
    Vector3 idleAttackRot = new Vector3(-31.648f, -174.941f, 252.741f);

    Vector3 idleCleanPos = new Vector3(0.626f, -1.753f, -0.14f);
    Vector3 idleCleanRot = new Vector3(39.455f, -186.801f, 65.83f);

    Vector3 cleaningPos = new Vector3(0.9141538f, -0.2715257f, -1.712738f);
    Vector3 cleaningRot = new Vector3(38.529f, -164.662f, 349.231f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateMop(bool isAttackMode, bool isCleaning)
    {
        if (isAttackMode)
        {
            transform.localPosition = idleAttackPos;
            transform.localRotation = Quaternion.Euler(idleAttackRot.x, idleAttackRot.y, idleAttackRot.z);
        }
        else if (isCleaning)
        {
            Debug.Log("hi");
            transform.localPosition = cleaningPos;
            transform.localRotation = Quaternion.Euler(cleaningRot.x, cleaningRot.y, cleaningRot.z);
        }
        else
        {
            transform.localPosition = idleCleanPos;
            transform.localRotation = Quaternion.Euler(idleCleanRot.x, idleCleanRot.y, idleCleanRot.z);
        }
    }
}
