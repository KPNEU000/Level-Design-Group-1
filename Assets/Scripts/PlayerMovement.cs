using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    public static bool sprinting = false;

    [Header("Controls")]
    public float speed = 0.8f;
    public float jumpHeight = 0.2f;
    public float defaultSpeed;
    public float gravity = 9.81f;
    public float airControl = 10f;

    Vector3 input;
    Vector3 moveDirection;
    CharacterController controller;

    bool grounded;
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
        defaultSpeed = speed;
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        input = transform.right * moveHorizontal + transform.forward * moveVertical;
        input.Normalize();

        if (controller.isGrounded)
        {
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
}
