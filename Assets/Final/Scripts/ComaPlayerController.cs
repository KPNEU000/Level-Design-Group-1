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
    [SerializeField] float bobFrequency = 8f;
    [SerializeField] float bobReturnSpeed = 6f;
    [SerializeField] float bobReturnSpeedMin = 0.8f;

    [Header("Walk Zoom")]
    [SerializeField] float walkFOVOffset = -3f;
    [SerializeField] float fovLerpSpeed = 4f;

    [Header("Hit Feedback")]
    [SerializeField] float slowDuration = 2f;
    [SerializeField] float slowMultiplier = 0.5f;
    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeMagnitude = 0.05f;

    [Header("Terrain Slowdown")]
    [SerializeField] Terrain terrain;
    [SerializeField] float terrainCheckRadius = 2f;
    [SerializeField] float maxTerrainSlow = 0.4f; // 0 = full stop, 1 = no slow
    [SerializeField] int detailLayerIndex = 0;

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
            float terrainDensity = GetTerrainDensity();
            float terrainMultiplier = Mathf.Lerp(1f, maxTerrainSlow, terrainDensity);

            // combine hit slow + terrain slow
            float hitMultiplier = (slowTimer > 0f ? slowMultiplier : 1f);

            float speed = moveSpeed * hitMultiplier * terrainMultiplier;
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

        // At low health, the bob drags back to center very slowly
        float effectiveReturnSpeed = Mathf.Lerp(bobReturnSpeedMin, bobReturnSpeed, healthRatio);

        if (moving)
        {
            bobTimer += bobFrequency * Time.deltaTime;
            currentBobY = Mathf.Sin(bobTimer) * bobAmplitude;
            currentBobX = Mathf.Sin(bobTimer * 0.5f) * bobAmplitude * 0.5f;
            if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFOV + walkFOVOffset, fovLerpSpeed * Time.deltaTime);
        }
        else
        {
            bobTimer = 0f;
            currentBobY = Mathf.Lerp(currentBobY, 0f, effectiveReturnSpeed * Time.deltaTime);
            currentBobX = Mathf.Lerp(currentBobX, 0f, effectiveReturnSpeed * Time.deltaTime);
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

    float GetTerrainDensity()
    {
        if (terrain == null) return 0f;

        TerrainData data = terrain.terrainData;

        Vector3 terrainPos = transform.position - terrain.transform.position;

        int mapX = Mathf.RoundToInt((terrainPos.x / data.size.x) * data.detailWidth);
        int mapZ = Mathf.RoundToInt((terrainPos.z / data.size.z) * data.detailHeight);

        int radius = 2;

        float total = 0f;
        int count = 0;

        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                int px = Mathf.Clamp(mapX + x, 0, data.detailWidth - 1);
                int pz = Mathf.Clamp(mapZ + z, 0, data.detailHeight - 1);

                int[,] samples = data.GetDetailLayer(px, pz, 1, 1, detailLayerIndex);
                total += samples[0, 0];
                count++;
            }
        }

        if (count == 0) return 0f;

        // normalize (rough scaling — tweak if needed)
        return Mathf.Clamp01(total / (count * 16f));
    }
}
