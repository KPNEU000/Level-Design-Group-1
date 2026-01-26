using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public float explosinForce = 20;
    public float explosionRadius = 20;
    public float throwForce = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.AddExplosionForce(explosinForce,transform.position,explosionRadius);
    }
}
