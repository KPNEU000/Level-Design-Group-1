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
    public bool playerIsCloseEnough = false;
    public GameObject currentInteractableObject = null;
    private bool canMove = true;

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
        if (canMove)
            playerTransform.rotation = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up);


        if (canMove)
        {
            //regular movement and running
            if (Input.GetKey(KeyCode.LeftShift))
            {
                controller.Move(movement * runSpeed * Time.deltaTime);
                playerAnimator.SetBool("IsRunning", true);
                playerAnimator.SetBool("IsIdle", false);
                playerAnimator.SetBool("IsWalking", false);
            }
            else
            {
                controller.Move(movement * speed * Time.deltaTime);
                playerAnimator.SetBool("IsRunning", false);
                if (movement.magnitude > 0)
                {
                    playerAnimator.SetBool("IsWalking", true);
                    playerAnimator.SetBool("IsIdle", false);
                }
                else
                {
                    playerAnimator.SetBool("IsWalking", false);
                    playerAnimator.SetBool("IsIdle", true);
                }
            }
        }


        if (playerIsCloseEnough && Input.GetKeyDown(KeyCode.E))
        {
            if (!canMove) return;

            if (currentInteractableObject != null && currentInteractableObject.CompareTag("Lock"))
            {
                canMove = false;
                Vector3 targetPositionAdjusted = new Vector3(currentInteractableObject.transform.position.x,
                                             transform.position.y,
                                             currentInteractableObject.transform.position.z);

                transform.LookAt(targetPositionAdjusted);

                playerAnimator.SetBool("IsWalking", false);
                playerAnimator.SetBool("IsIdle", false);
                playerAnimator.SetBool("IsRunning", false);
                playerAnimator.SetBool("IsInteractingLock", true);
                playerAnimator.SetBool("IsFailLock", true);
            }
        }
    }

    public void WhenShrugEnd()
    {
        playerAnimator.SetBool("IsInteractingLock", false);
        playerAnimator.SetBool("IsFailLock", false);
        canMove = true;
    }
}
