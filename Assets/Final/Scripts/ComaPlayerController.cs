using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComaPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.8f;

    [Header("Jumping")]
    [SerializeField] float jumpHeight = 1.5f;

    [Header("Look")]
    [SerializeField] Transform cameraFollowTarget;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float verticalClamp = 80f;

    CharacterController cc;
    Animator anim;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction sprintAction;

    Vector3 velocity;
    float xRotation;
    bool jumpQueued;

    float gravity = -9.81f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];

        jumpAction.performed += _ => jumpQueued = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        changeAnim("isIdle");
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        Debug.Log("lookInput: " + lookInput);

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        Debug.Log("xRotation: " + xRotation);

        cameraFollowTarget.rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);
    }

    void HandleMovement()
    {
        bool grounded = cc.isGrounded;

        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isSprinting = sprintAction.ReadValue<float>() > 0.5f;

        float speed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        cc.Move(move * speed * Time.deltaTime);

        if (jumpQueued && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpQueued = false;
        }
        else
        {
            jumpQueued = false;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        if (isSprinting)
        {
            changeAnim("isSprinting");
        }
        else if (move.magnitude > 0.1f)
        {
            changeAnim("isWalking");
        }
        else changeAnim("isIdle");
    }

    void changeAnim(string animBool)
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isWalking", false);
        anim.SetBool("isSprinting", false);

        if (animBool != "none")
            anim.SetBool(animBool, true);
    }
}