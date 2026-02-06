using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Controls")]
    public float speed = 10f;
    public float jumpHeight = 0.5f;
    public float gravity = 9.81f;
    public float airControl = 10f;

    Vector3 input;
    Vector3 moveDirection;
    CharacterController controller;

    Animator animator;

    private AudioSource playerAudioSource;

    [Header("Audio")]
    public AudioClip genericWalkSFX;

    bool grounded;
    public float range = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        animator = GetComponent<Animator>();
        playerAudioSource = GetComponent<AudioSource>();
        UpdatePlayerAnim(0);
        InvokeRepeating("PlayWalkSound", 0, 0.2f);
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        input = transform.right * moveHorizontal + transform.forward * moveVertical;
        input.Normalize();

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

    public void PlayWalkSound()
    {
        if (controller.isGrounded && input != Vector3.zero)
        {
            playerAudioSource.pitch = UnityEngine.Random.Range(0, 5);
            playerAudioSource.PlayOneShot(genericWalkSFX);
        }
    }


    public void UpdatePlayerAnim(int animState)
    {
        animator.SetInteger("animState", animState);
    }
}
