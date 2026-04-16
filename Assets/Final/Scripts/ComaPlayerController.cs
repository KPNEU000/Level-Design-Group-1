using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComaPlayerController : MonoBehaviour
{

    [SerializeField] Transform respawnPoint;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Look")]
    [SerializeField] Transform cameraFollowTarget;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] float verticalClamp = 80f;

    [Header("Walker / Body Lag")]
    [SerializeField] float lookFreedom = 40f;
    [SerializeField] float bodyRotateSpeed = 60f;

    CharacterController cc;
    Animator anim;
    PlayerInput playerInput;
    InputAction lookAction;

    Vector3 velocity;
    float xRotation;
    float gravity = -9.81f;

    float lookYaw;
    float bodyYaw;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookYaw = transform.eulerAngles.y;
        bodyYaw = lookYaw;
    }

    void Start()
    {
        GameManager.Ins.OnDeath += Death;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        lookYaw += lookInput.x * mouseSensitivity;

        bodyYaw = (bodyYaw % 360f + 360f) % 360f;
        lookYaw = (lookYaw % 360f + 360f) % 360f;

        transform.rotation = Quaternion.Euler(0f, bodyYaw, 0f);

        float mouseY = lookInput.y * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        cameraFollowTarget.rotation = Quaternion.Euler(xRotation, lookYaw, 0f);
    }

    void HandleMovement()
    {
        bool grounded = cc.isGrounded;

        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        bool moving = Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;

        float yawDelta = Mathf.DeltaAngle(bodyYaw, lookYaw);
        bool withinCone = Mathf.Abs(yawDelta) <= lookFreedom;

        float rotateSpeed = (moving && !withinCone) ? bodyRotateSpeed * 3f : bodyRotateSpeed;
        bodyYaw = Mathf.MoveTowardsAngle(bodyYaw, lookYaw, rotateSpeed * Time.deltaTime);

        if (moving && withinCone)
        {
            Vector3 moveForward = Quaternion.Euler(0f, lookYaw, 0f) * Vector3.forward;
            cc.Move(moveForward * moveSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        //debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Ins.RemoveHealth(10);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            GameManager.Ins.OnDeath();
        }
    }

    void Death()
    {
        transform.position = respawnPoint.position;
        GameManager.Ins.AfterRespawn();
    }
}