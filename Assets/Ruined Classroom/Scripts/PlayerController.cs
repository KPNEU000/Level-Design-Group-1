using UnityEngine;
using System.Collections;

class PlayerController : MonoBehaviour
{
    Animator anim;
    public AudioClip[] numberSounds = new AudioClip[3];
    public AudioSource clipAudioSource;
    public AudioSource damageAudioSource;
    public AudioSource breathingAudioSource;

    public OxygenController oxygenController;

    public bool isCrouching = false;
    bool locked = false;

    float mouseSensitivity = 2f;
    float pitch = 0f;

    int moveSpeed = 2;

    Vector3 standingCameraPos;
    Vector3 crouchingCameraPos;

    Coroutine headCoroutine;

    CharacterController controller;
    float standingColliderCenterY = 2.5f;
    float standingColliderHeight = 5.5f;
    float crouchingColliderCenterY = 1.3f;
    float crouchingColliderHeight = 3f;

    public Transform head;

    void Start()
    {
        standingCameraPos = new Vector3(-.12f, 4.694f, 0.335f);
        crouchingCameraPos = new Vector3(.36f, 2.35f, 1.3f);
        head.localPosition = standingCameraPos;

        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        controller.center = new Vector3(0, standingColliderCenterY, 0);
        controller.height = standingColliderHeight;

    }

    void Update()
    {
        HandleMouseLook();
        HandleCrouch();
        HandleMovement();
        HandleInteract();
        //HandleNumberKeys();
    }

    void HandleMovement()
    {
        if (locked) return;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * v + right * h;

        bool isMoving = move.magnitude > 0;

        //update anims
        if (isCrouching)
        {
            anim.SetBool("IsCrouchingMoving", isMoving);
            anim.SetBool("IsCrouchingIdle", !isMoving);
        }
        else
        {
            anim.SetBool("IsStandingMoving", isMoving);
            anim.SetBool("IsStandingIdle", !isMoving);
        }

        //normalize diagonal
        if (move.magnitude > 1)
            move.Normalize();

        if (isCrouching)
        {
            controller.Move(move * (moveSpeed * 0.5f) * Time.deltaTime);
        }
        else
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
        head.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    void HandleCrouch()
    {
        if (locked) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            locked = true;

            if (!isCrouching)
            {
                anim.SetBool("IsStandingMoving", false);
                anim.SetBool("IsStandingIdle", false);
                anim.SetBool("IsStandToCrouch", true);
                anim.SetBool("IsCrouchToStand", false);
                MoveHeadTo(crouchingCameraPos, .1f);
            }
            else
            {
                anim.SetBool("IsCrouchingMoving", false);
                anim.SetBool("IsCrouchingIdle", false);
                anim.SetBool("IsCrouchToStand", true);
                anim.SetBool("IsStandToCrouch", false);
                MoveHeadTo(standingCameraPos, .7f);
            }
        }
    }

    void HandleInteract()
    {
        if (locked) return;

        if (Input.GetKey(KeyCode.E))
        {
            locked = true;
            anim.SetBool("IsStandingInteract", true);
        }
        else
        {
            anim.SetBool("IsStandingInteract", false);
        }
    }

    public void ExitCrouchToStand()
    {
        //locked = false;
        isCrouching = false;
        anim.SetBool("IsCrouchToStand", false);
        SetColliderSettings(true);
        anim.SetBool("IsCrouchingMoving", false);
        anim.SetBool("IsCrouchingIdle", false);
    }

    public void ExitStandToCrouch()
    {
        //locked = false;
        isCrouching = true;
        anim.SetBool("IsStandToCrouch", false);
        SetColliderSettings(false);
        anim.SetBool("IsStandingMoving", false);
        anim.SetBool("IsStandingIdle", false);

    }

    public void ExitStandInteract()
    {
        //locked = false;
    }

    void MoveHeadTo(Vector3 target, float delay = 0f)
    {
        if (headCoroutine != null)
            StopCoroutine(headCoroutine);
        headCoroutine = StartCoroutine(WaitToMoveHead(target, delay));
    }

    IEnumerator WaitToMoveHead(Vector3 target, float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(MoveHead(target));
    }

    IEnumerator MoveHead(Vector3 target)
    {
        float duration = 1f;
        Vector3 start = head.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            head.localPosition = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        head.localPosition = target;
        locked = false;
    }

    void HandleNumberKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlayNumberSound(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlayNumberSound(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) PlayNumberSound(2);
    }

    void PlayNumberSound(int index)
    {
        clipAudioSource.clip = numberSounds[index];
        clipAudioSource.Play();
    }

    void SetColliderSettings(bool isStanding)
    {
        if (isStanding)
        {
            controller.center = new Vector3(0, standingColliderCenterY, 0);
            controller.height = standingColliderHeight;
        }
        else
        {
            controller.center = new Vector3(0, crouchingColliderCenterY, 0);
            controller.height = crouchingColliderHeight;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Damaging")) return;

        damageAudioSource.Play();
        breathingAudioSource.Play();
        oxygenController.ChangeOxygen(-10);

    }
}