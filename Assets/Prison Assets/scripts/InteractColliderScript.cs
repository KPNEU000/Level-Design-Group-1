using UnityEngine;

public class InteractColliderScript : MonoBehaviour
{
    public PlayerController pController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hiiii");
        if (other.CompareTag("Lock"))
        {
            pController.playerIsCloseEnough = true;
            pController.currentInteractableObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lock"))
        {
            pController.playerIsCloseEnough = false;
            pController.currentInteractableObject = null;
        }
    }
}
