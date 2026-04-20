using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
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

    [Header("Head Bob")]
    [SerializeField] float bobAmplitude = 0.05f;
    [SerializeField] float bobAmplitudeMax = 0.15f;
    [SerializeField] float bobFrequency = 8f;
    [SerializeField] float bobFrequencyMax = 11f;
    [SerializeField] float bobReturnSpeed = 6f;

    [Header("Walk Zoom")]
    [SerializeField] float walkFOVOffset = -3f;
    [SerializeField] float fovLerpSpeed = 4f;

    [Header("Hit Feedback")]
    [SerializeField] float slowDuration = 2f;
    [SerializeField] float slowMultiplier = 0.5f;
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeMagnitude = 0.05f;

    // Components
    CharacterController cc;
    Animator anim;
    PlayerInput playerInput;
    InputAction lookAction;
    Camera cam;

    // Look
    float lookYaw;
    float bodyYaw;
    float xRotation;

    // Movement
    Vector3 velocity;
    float gravity = -9.81f;
    bool moving;

    // Head bob
    Vector3 camRestLocalPos;
    float bobTimer;
    float currentBobY;
    float currentBobX;
    float baseFOV;

    // Hit feedback
    float slowTimer;
    float shakeTimer;
    Vector3 shakeOffset;

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

        camRestLocalPos = cameraFollowTarget.localPosition;
        cam = cameraFollowTarget.GetComponentInChildren<Camera>();
        cam = cam != null ? cam : Camera.main;
        if (cam != null) baseFOV = cam.fieldOfView;
    }

    void Start()
    {
        GameManager.Ins.OnDeath += Death;
        GameManager.Ins.OnDamaged += OnHit;
        transform.position = respawnPoint.position;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleHeadBob();
        HandleCameraShake();
    }

    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        lookYaw += lookInput.x * mouseSensitivity;
        bodyYaw = (bodyYaw % 360f + 360f) % 360f;
        lookYaw = (lookYaw % 360f + 360f) % 360f;

        transform.rotation = Quaternion.Euler(0f, bodyYaw, 0f);

        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        cameraFollowTarget.rotation = Quaternion.Euler(xRotation, lookYaw, 0f);
    }

    void HandleMovement()
    {
        if (cc.isGrounded && velocity.y < 0f) velocity.y = -2f;

        moving = Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;

        float yawDelta = Mathf.DeltaAngle(bodyYaw, lookYaw);
        bool withinCone = Mathf.Abs(yawDelta) <= lookFreedom;

        float rotateSpeed = (moving && !withinCone) ? bodyRotateSpeed * 3f : bodyRotateSpeed;
        bodyYaw = Mathf.MoveTowardsAngle(bodyYaw, lookYaw, rotateSpeed * Time.deltaTime);

        if (moving && withinCone)
        {
            float speed = moveSpeed * (slowTimer > 0f ? slowMultiplier : 1f);
            Vector3 moveForward = Quaternion.Euler(0f, lookYaw, 0f) * Vector3.forward;
            cc.Move(speed * Time.deltaTime * moveForward);
        }

        if (slowTimer > 0f) slowTimer -= Time.deltaTime;

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) GameManager.Ins.RemoveHealth(10); //for debugging
    }

    void HandleHeadBob()
    {
        float healthRatio = (float)GameManager.Ins.CurrentHealth / GameManager.Ins.MaxHealth;
        float effectiveAmplitude = Mathf.Lerp(bobAmplitudeMax, bobAmplitude, healthRatio);
        float effectiveFrequency = Mathf.Lerp(bobFrequencyMax, bobFrequency, healthRatio);

        if (moving)
        {
            bobTimer += effectiveFrequency * Time.deltaTime;
            currentBobY = Mathf.Sin(bobTimer) * effectiveAmplitude;
            currentBobX = Mathf.Sin(bobTimer * 0.5f) * effectiveAmplitude * 0.5f;
            if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV + walkFOVOffset, fovLerpSpeed * Time.deltaTime);
        }
        else
        {
            bobTimer = 0f;
            currentBobY = Mathf.Lerp(currentBobY, 0f, bobReturnSpeed * Time.deltaTime);
            currentBobX = Mathf.Lerp(currentBobX, 0f, bobReturnSpeed * Time.deltaTime);
            if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, fovLerpSpeed * Time.deltaTime);
        }

        cameraFollowTarget.localPosition = camRestLocalPos + new Vector3(currentBobX, currentBobY, 0f) + shakeOffset;
    }

    void HandleCameraShake()
    {
        if (shakeTimer <= 0f)
        {
            shakeOffset = Vector3.zero;
            return;
        }

        shakeTimer -= Time.deltaTime;
        float progress = shakeTimer / shakeDuration;
        shakeOffset = Random.insideUnitSphere * shakeMagnitude * progress;
    }

    void OnHit()
    {
        slowTimer = slowDuration;
        shakeTimer = shakeDuration;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death")) GameManager.Ins.OnDeath();
    }

    void Death()
    {
        slowTimer = 0f;
        shakeTimer = 0f;
        transform.position = respawnPoint.position;
        GameManager.Ins.AfterRespawn();
    }
}
