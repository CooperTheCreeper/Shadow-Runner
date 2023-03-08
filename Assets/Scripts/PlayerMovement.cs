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
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;

    void Start()
    {
        //give access to rigid and animator even though they are hidden, for cleanup
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        CheckCollision();
        AnimatorControllers();

        if (playerUnlocked && !wallDetected)
            //set movement based off of public values
            Movement();

        if (isGrounded)
        {
            canDoubleJump = true;
        }


        CheckInput();

    }

    private void Movement()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
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
        //check for wall interaction
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
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
        //Set up a wall check
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
