using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AcidSpew : MonoBehaviour
{
    public float throwForce = 65;
    public Rigidbody rb;
    public float damage = 10;
    public GameObject goopyMess;
    public AudioSource audioSource;
    public AudioClip squish;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(squish);
        if(collision.gameObject.GetComponent<Health>())
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
        Instantiate(goopyMess, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
