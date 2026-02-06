using UnityEngine;
using UnityEngine.XR;

public class EdibleObject : MonoBehaviour
{
    public bool held = false;
    public Rigidbody rb;
    public float speed = 0.1f;
    public float damage = 10;
    public enum Type {organic, inorganic};
    public GameObject hand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (held)
        {
            rb.useGravity = false;
            rb.position = Vector3.MoveTowards(rb.position, hand.transform.position, Time.deltaTime * speed);
        }
        else
        {
            rb.useGravity = true;
        }
    }
}
