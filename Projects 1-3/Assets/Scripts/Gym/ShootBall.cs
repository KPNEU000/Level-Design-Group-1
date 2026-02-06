using Unity.VisualScripting;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    public GameObject ball;
    GameObject clone;
    public float throwForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(ball, transform.position + transform.forward, transform.rotation);
        }
    }
}
