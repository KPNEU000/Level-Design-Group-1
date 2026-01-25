using UnityEngine;

public class JanitorMovement : MonoBehaviour
{
    public static JanitorMovement Instance;
    public Animator animator;
    public static bool sprinting = false;

    [Header("Controls")]
    public float speed = 0.8f;
    public float jumpHeight = 0.5f;
    public float defaultSpeed;
    public float gravity = 9.81f;
    public float airControl = 10f;

    Vector3 input;
    Vector3 moveDirection;
    CharacterController controller;

    bool grounded;
    public bool isAttackMode = false;

    KeyCode modeSwitchKey = KeyCode.E;
    public float range = 1;
    private void Awake()
    {
        if (Instance != null & Instance != this) //If there is another Instance
        {
            Destroy(gameObject); //Destroy this one so there is only one
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        isAttackMode = false;
        defaultSpeed = speed;
    }

    void Update()
    {
        Move();
        ModeHandler();
    }

    public void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        input = transform.right * moveHorizontal + transform.forward * moveVertical;
        input.Normalize();

        if (controller.isGrounded)
        {
            if (Mathf.Abs(moveHorizontal) > 0.001f || Mathf.Abs(moveVertical) > 0.001f)
            {
                HandleAnimationSwitch(true);
                animator.SetBool("IsMoveAttack", true);
                animator.SetBool("IsIdleAttack", false);
            }
            else
            {
                animator.SetBool("IsIdleAttack", true);
                animator.SetBool("IsMoveAttack", false);
            }

            moveDirection = input;
            moveDirection.y = 0.0f;
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            moveDirection = input;
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(2 * jumpHeight * gravity);
            }
            else
            {
                moveDirection.y = 0.0f;
            }
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }
        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(input * speed * Time.deltaTime);
    }

    void ModeHandler()
    {
        if (Input.GetKey(modeSwitchKey))
        {
            if (isAttackMode)
            {
                isAttackMode = false;
            }
            else
            {
                isAttackMode = true;
            }
        }
    }

    void HandleAnimationSwitch(bool nowMoving)
    {
        if (nowMoving)
        {
            if (animator.GetBool("IsIdleAttack"))
            {
                animator.SetBool("IsMoveAttack", true);
                animator.SetBool("IsIdleAttack", false);
            }
            else if (animator.GetBool("IsIdleClean"))
            {
                animator.SetBool("IsMoveClean", true);
                animator.SetBool("IsIdleClean", false);
            }
        }
        else
        {
            if (animator.GetBool("IsMoveAttack"))
            {
                animator.SetBool("IsMoveAttack", false);
                animator.SetBool("IsIdleAttack", true);
            }
            else if (animator.GetBool("IsMoveClean"))
            {
                animator.SetBool("IsMoveClean", false);
                animator.SetBool("IsIdleClean", true);
            }
        }

    }
}