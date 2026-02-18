using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Insert Character Controller")]
    private CharacterController controller;

    [SerializeField]
    [Tooltip("Insert Camera Controller")]
    private Camera mainCamera;

    [SerializeField]
    [Tooltip("Insert Player Animator")]
    private Animator playerAnimator;

    private Vector3 velocity;
    public float speed = 2f;
    public float runSpeed = 6f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //  Grab transforms
        Transform playerTransform = transform;
        Transform cameraTransform = mainCamera.transform;


        // Ground movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = (playerTransform.right * x) + (playerTransform.forward * z);
        //Vector3 movement = (playerTransform.forward * z);


        // Player rotate alongside camera
        playerTransform.rotation = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up);


        //regular movement and running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(movement * runSpeed * Time.deltaTime);
            playerAnimator.SetBool("IsRunning", true);
        }
        else
        {
            controller.Move(movement * speed * Time.deltaTime);
            playerAnimator.SetBool("IsRunning", false);
        }


        if (movement.magnitude > 0)
        {
            playerAnimator.SetBool("IsWalking", true);
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
        }


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1))
            {
                if (hit.collider.CompareTag("Lock"))
                {
                    Debug.Log("Clicked");
                }
            }
        }
    }
}
