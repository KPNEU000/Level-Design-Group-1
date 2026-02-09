using System;
using TMPro;
using UnityEngine;


public class PlayerInteraction : MonoBehaviour
{
    public float range = 10;
    public AudioClip eatSound;
    public AudioSource playerAudioSource;
    public GameObject heldObject = null;
    public Health playerHealth;
    public bool throwing;
    public TextMeshProUGUI text;

    [SerializeField]
    PlayerMovement playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerHealth = transform.parent.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastingEffect();
        if(Input.GetKey(KeyCode.LeftShift))
        {
            throwing = true;
        }
        else
        {
            throwing = false;
        }
        /*
        if (heldObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Eat(heldObject.GetComponent<EdibleObject>());
            }
            else if (Input.GetMouseButtonUp(0))
            {
                heldObject.GetComponent<EdibleObject>().held = true;
                heldObject = null;
            }
        }
        */
    }

    void RaycastingEffect()
    {
        RaycastHit objectHitByRaycast;
        if (Physics.Raycast(transform.position, transform.forward, out objectHitByRaycast, range))
        {
            if (objectHitByRaycast.collider.CompareTag("Edible"))
            {
                //inspect.text = objectHitByRaycast.transform.name;
                text.text = objectHitByRaycast.collider.gameObject.name;
                if (Input.GetMouseButton(0)) //Could split into two checks. 1 for when you start holding, 2 for when you keep holding
                {
                    objectHitByRaycast.collider.gameObject.GetComponent<EdibleObject>().held = true;
                    heldObject = objectHitByRaycast.collider.gameObject;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                    Eat(heldObject.GetComponent<EdibleObject>());
                    }
                }
                else
                {
                    heldObject.GetComponent<EdibleObject>().held = false;
                    if(throwing)
                    {
                        heldObject.GetComponent<Rigidbody>().AddForce(Vector3.forward * 200);
                    }
                    heldObject = null;
                }
            }
            else
            {
                text.text = "";
            }
        }
    }

    private void Eat(EdibleObject edibleObject)
    {
        playerAudioSource.PlayOneShot(eatSound);
        Destroy(edibleObject.gameObject);
        playerHealth.TakeDamage(edibleObject.damage);
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}
