using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //ref to rigidbody
    private Rigidbody2D rb;

    //access animator controller
    private Animator anim;

    //movement and jump
    [Header("Movement & Jumping")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    //double jump
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;
    

    //bool for movement
    private bool playerUnlocked;

    //check for collision with ground
    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    void Start()
    {
        //give access to rigid and animator even though they are hidden, for cleanup
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        AnimatorControllers();

        if (playerUnlocked)
            //set movement based off of public values
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        CheckCollision();

        CheckInput();

    }

    private void AnimatorControllers()
    {
        //control blend tree animation for jump/fall & idle/move
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void CheckCollision()
    {
        //check if player is contacting ground
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Right"))
            //set button for movement
            playerUnlocked = true;

        //Set button for jump
        if (Input.GetButtonDown("Jump"))
            JumpButton();
    }

    private void JumpButton()
    {
        if (isGrounded)
        {
            canDoubleJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
        }
        
    }

    private void OnDrawGizmos()
    {
        //Line to check ground check distance variable
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
