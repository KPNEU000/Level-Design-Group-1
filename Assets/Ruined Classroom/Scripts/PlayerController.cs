using UnityEngine;

class PlayerController : MonoBehaviour
{
    Animator anim;

    bool isCrouching = false;
    bool locked = false;

    float mouseSensitivity = 2f;
    float pitch = 0f;

    int moveSpeed = 2;

    CharacterController controller;

    public Transform head;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMouseLook();
        HandleCrouch();
        HandleMovement();
        HandleInteract();
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

        controller.Move(move * moveSpeed * Time.deltaTime);
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
            }
            else
            {
                anim.SetBool("IsCrouchingMoving", false);
                anim.SetBool("IsCrouchingIdle", false);
                anim.SetBool("IsCrouchToStand", true);
                anim.SetBool("IsStandToCrouch", false);
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
        locked = false;
        isCrouching = false;
        anim.SetBool("IsCrouchToStand", false);
    }

    public void ExitStandToCrouch()
    {
        locked = false;
        isCrouching = true;
        anim.SetBool("IsStandToCrouch", false);

    }

    public void ExitStandInteract()
    {
        locked = false;
    }
}