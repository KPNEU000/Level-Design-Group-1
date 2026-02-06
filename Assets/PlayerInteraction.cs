using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{

    public float range = 10;

    [SerializeField]
    PlayerMovement playerMovement;

    [Header("HUD")]
    public TMP_Text inspect;
    public TMP_Text keyInventory;
    public TMP_Text clueInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastingEffect();
    }

    void RaycastingEffect()
    {
        RaycastHit objectHitByRaycast;
        if (Physics.Raycast(transform.position, transform.forward, out objectHitByRaycast, range))
        {
            if (objectHitByRaycast.collider.CompareTag("EdibleObject"))
            {
                inspect.text = objectHitByRaycast.transform.name;
                if (Input.GetButtonDown("Jump"))
                {
                    objectHitByRaycast.GetComponent<EdibleObject>().held = true;
                }
                else
                {
                    objectHitByRaycast.GetComponent<EdibleObject>().held = false;
                }
            }
            else
            {
                inspect.text = "";
            }
        }
    }

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }
}
