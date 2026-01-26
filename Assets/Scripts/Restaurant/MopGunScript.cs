using UnityEngine;

public class MopGunScript : MonoBehaviour
{

    public GameObject character;
    public GameObject bone;


    Vector3 idleAttackPos = new Vector3(-1.102617f, 1.752648f, -0.3680f);
    Vector3 idleAttackRot = new Vector3(-31.566f, -170.472f, 249.595f);

    Vector3 idleCleanPos = new Vector3(1.067018f, -1.556953f, -0.16059f);
    Vector3 idleCleanRot = new Vector3(38.817f, -553.733f, 53.748f);

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
