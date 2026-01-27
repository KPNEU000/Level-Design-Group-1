using UnityEngine;

public class MopGunScript : MonoBehaviour
{

    public GameObject character;
    public GameObject bone;


    Vector3 idleAttackPos = new Vector3(0.015f, 0.086f, 0.024f);
    Vector3 idleAttackRot = new Vector3(17.558f, -88.404f, -33.199f);

    Vector3 idleCleanPos = new Vector3(-0.226f, 0.021f, -0.03f);
    Vector3 idleCleanRot = new Vector3(-18.458f, -285.51f, 234.372f);

    Vector3 cleaningPos = new Vector3(0.2307734f, -0.2744828f, 0.1886233f);
    Vector3 cleaningRot = new Vector3(-54.347f, -126.878f, -203.88f);
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
