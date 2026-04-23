using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    [SerializeField] float bobAmplitude = 0.03f;
    [SerializeField] float bobAmplitudeMax = 0.08f;
    [SerializeField] float bobFrequency = 6f;
    [SerializeField] float bobFrequencyMax = 8.5f;
    [SerializeField] float bobReturnSpeed = 6f;

    [Header("Walk Zoom")]
    [SerializeField] float walkFOVOffset = -3f;
    [SerializeField] float fovLerpSpeed = 4f;

    [Header("Hit Feedback")]
    [SerializeField] float slowDuration = 2f;
    [SerializeField] float slowMultiplier = 0.5f;
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeMagnitude = 0.05f;

    [Header("Death Animation")]
    [SerializeField] float deathFallDuration = 0.8f;
    [SerializeField] float deathFallAngle = 80f;       //how far forward it tips
    [SerializeField] float deathFallSideAngle = 15f;   //slight sideways lean
    [SerializeField] float deathDropHeight = 0.6f;     //how far camera drops
    [SerializeField] float deathBounceHeight = 0.05f;  //small bounce on impact
    [SerializeField] float deathBounceDuration = 0.2f;
    [SerializeField] float fadeDuration = 1f;
    bool isDead = false;


    [Header("Drooping")]
    [SerializeField] float droopAmount = -0.08f;
    [SerializeField] float droopRecoverBase = 0.4f;
    [SerializeField] float droopRecoverMin = 0.05f;

    float droopOffset = 0f;
    float droopTarget = 0f;

    [SerializeField] GameObject walker;



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

    private bool hasFaded = false;
    bool isInitialized = false;

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

    void OnHit()
    {
        slowTimer = slowDuration;
        shakeTimer = shakeDuration;
        droopOffset = droopAmount; // snap downward instantly on hit
    }


    void Update()
    {
        if (isDead) return;
        HandleLook();
        HandleMovement();
        HandleHeadBob();
        HandleCameraShake();
        // ... your space key test etc


        if (!hasFaded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            hasFaded = true;
            Debug.Log("gucci bro");
            Utils.StartFade(this, walker, 1, 0, 2);
        }
    }


    void HandleLook()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        lookYaw += lookInput.x * mouseSensitivity;
        bodyYaw = (bodyYaw % 360f + 360f) % 360f;
        lookYaw = (lookYaw % 360f + 360f) % 360f;

        transform.rotation = Quaternion.Euler(0f, bodyYaw, 0f);

        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, isDead ? 90f : verticalClamp);
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
    }

    void HandleHeadBob()
    {
        float healthRatio = (float)GameManager.Ins.CurrentHealth / GameManager.Ins.MaxHealth;

        float effectiveAmplitude = Mathf.Lerp(bobAmplitudeMax, bobAmplitude, healthRatio);
        float effectiveFrequency = Mathf.Lerp(bobFrequencyMax, bobFrequency, healthRatio);

        float recoverSpeed = Mathf.Lerp(droopRecoverMin, droopRecoverBase, healthRatio);
        droopOffset = Mathf.Lerp(droopOffset, 0f, recoverSpeed * Time.deltaTime);

        if (moving)
        {
            // At low health: faster downswing, slower upswing
            float downSpeed = Mathf.Lerp(bobFrequency, bobFrequency * 2.5f, 1f - healthRatio);
            float upSpeed = Mathf.Lerp(bobFrequency, bobFrequency * 0.4f, 1f - healthRatio);

            float speed = (Mathf.Sin(bobTimer) < 0f) ? downSpeed : upSpeed;
            bobTimer += speed * Time.deltaTime;

            currentBobY = Mathf.Sin(bobTimer) * bobAmplitude;
            currentBobX = Mathf.Sin(bobTimer * 0.5f) * bobAmplitude * 0.5f;
            if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV + walkFOVOffset, fovLerpSpeed * Time.deltaTime);
        }
        else
        {
            bobTimer = Mathf.PI * 1.5f;
            currentBobY = Mathf.Lerp(currentBobY, 0f, bobReturnSpeed * Time.deltaTime);
            currentBobX = Mathf.Lerp(currentBobX, 0f, bobReturnSpeed * Time.deltaTime);
            if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV, fovLerpSpeed * Time.deltaTime);
        }

        cameraFollowTarget.localPosition = camRestLocalPos
            + new Vector3(currentBobX, currentBobY + droopOffset, 0f)
            + shakeOffset;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death")) GameManager.Ins.OnDeath();
    }

    void Death()
    {
        if (isDead) return;
        isDead = true;
        slowTimer = 0f;
        shakeTimer = 0f;
        droopOffset = 0f;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        float timer = 0f;

        float startXRotation = xRotation;
        float startLookYaw = lookYaw;

        float targetXRotation = 85f; // looking straight down at ground
        float targetLookYaw = lookYaw + deathFallSideAngle;

        // --- Phase 1: Fall ---
        while (timer < deathFallDuration)
        {
            timer += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(timer / deathFallDuration), 2f);
            xRotation = Mathf.Lerp(startXRotation, targetXRotation, t);
            lookYaw = Mathf.Lerp(startLookYaw, targetLookYaw, t);
            cameraFollowTarget.rotation = Quaternion.Euler(xRotation, lookYaw, 0f);
            yield return null;
        }

        // --- Phase 2: Bounce ---
        timer = 0f;
        float bounceXRotation = targetXRotation - 5f;

        while (timer < deathBounceDuration * 0.5f)
        {
            timer += Time.deltaTime;
            float t = timer / (deathBounceDuration * 0.5f);
            xRotation = Mathf.Lerp(targetXRotation, bounceXRotation, t);
            cameraFollowTarget.rotation = Quaternion.Euler(xRotation, lookYaw, 0f);
            yield return null;
        }
        while (timer < deathBounceDuration)
        {
            timer += Time.deltaTime;
            float t = (timer - deathBounceDuration * 0.5f) / (deathBounceDuration * 0.5f);
            xRotation = Mathf.Lerp(bounceXRotation, targetXRotation, t);
            cameraFollowTarget.rotation = Quaternion.Euler(xRotation, lookYaw, 0f);
            yield return null;
        }

        // --- Phase 3: Fade to black ---
        yield return new WaitForSeconds(0.3f);
        if (ScreenFader.Ins != null)
            yield return StartCoroutine(ScreenFader.Ins.FadeTo(1f, fadeDuration));

        // --- Reset ---
        transform.position = respawnPoint.position;
        cameraFollowTarget.localPosition = camRestLocalPos;
        lookYaw = transform.eulerAngles.y;
        bodyYaw = lookYaw;
        xRotation = 0f;
        transform.rotation = Quaternion.Euler(0f, bodyYaw, 0f);
        cameraFollowTarget.rotation = Quaternion.Euler(0f, lookYaw, 0f);
        GameManager.Ins.AfterRespawn();

        // --- Phase 4: Fade back in ---
        if (ScreenFader.Ins != null)
            yield return StartCoroutine(ScreenFader.Ins.FadeTo(0f, fadeDuration));

        isDead = false;
    }
}
