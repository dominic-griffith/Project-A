using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //add reference in editor
    [Header("References")]
    public Rigidbody rb;
    public LayerMask groundMask;
    public CapsuleCollider playerBody;


    [Header("Movement")]
    public float walkSpeed = 70;
    public float sprintSpeed = 100;
    public MovementState movementState;
    public float groundDrag = 5f;

    private float x;
    private float z;
    private float speed;
    private float playerHeight;
    private bool isGrounded;
    

    [Header("Jumping")]
    public float jumpForce = 12f;
    public float jumpCoolDown = 0.25f;
    public float airMultiplier = 0.4f;

    private bool isJumping;


    [Header("Crouching")]
    public float crouchSpeed = 35f;
    public float crouchYScale = 0.5f;

    private float startYScale;



    private void Start()
    {
        playerHeight = playerBody.height;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        PlayerInput();

        SpeedControl();
        StateHandler();
        ApplyDrag();
    }

    private void FixedUpdate()
    {
        moveCharacter();
    }

    private void PlayerInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        //applys jump to player and also ensures they cant jump infinitely
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            isJumping = true;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }

        //starts crouch
        if (Input.GetButtonDown("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //stop crouch
        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void moveCharacter()
    {
        Vector3 move = transform.right * x + transform.forward * z;
        if (isGrounded)
            rb.AddForce(move.normalized * speed, ForceMode.Force);
        else
            rb.AddForce(move.normalized * speed * airMultiplier, ForceMode.Force);
    }

    private void ApplyDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVelocity.magnitude > speed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * speed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        //ensures we jump the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        isJumping = false;
    }

    private void StateHandler()
    {
        //Crouching
        if (Input.GetButton("Crouch"))
        {
            movementState = MovementState.crouching;
            speed = crouchSpeed;
        }
        //Sprinting
        if(isGrounded && Input.GetButton("Sprint"))
        {
            movementState = MovementState.sprinting;
            speed = sprintSpeed;
        }
        //Walking
        else if(isGrounded) {
            movementState = MovementState.walking;
            speed = walkSpeed;
        }
        //Air
        else
        {
            movementState = MovementState.air;
        }
    }

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }
}
