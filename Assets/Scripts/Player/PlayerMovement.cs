using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //add reference in editor
    public Rigidbody rb;
    public LayerMask groundMask;
    public CapsuleCollider playerBody;

    public float speed = 120f;
    public float groundDrag = 5f;
    public float jumpForce = 12f;
    public float jumpCoolDown = 0.25f;
    public float airMultiplier = 0.4f;

    private float playerHeight;
    private float x;
    private float z;
    private bool isGrounded;
    private bool isJumping;

    private void Start()
    {
        playerHeight = playerBody.height;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        SpeedControl();

        //get player input
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        ApplyDrag();

        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            isJumping = true;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    private void FixedUpdate()
    {
        moveCharacter();
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
}
