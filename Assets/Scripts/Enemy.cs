using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    //move the RB
    [Header("Movement Details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    //control movement
    public bool canMove;

    //---------------------------------------------------------------------------------------

    //check for collision with ground
    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceilingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundForwardCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool groundForward;
    private bool wallDetected;
    private bool ceilingDetected;

    //Ledge grabbing
    public bool ledgeDetected;

    //Freeze enemy at ledgecheck position
    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1; //offset for position BEFORE climb
    [SerializeField] private Vector2 offset2; //offset for position AFTER climb

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;

    //---------------------------------------------------------------------------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckCollision();
        AnimatorControllers();
        Movement();
        CheckForLedge();

        //have enemy jump if there is no ground on the check
        if(isGrounded && !groundForward || wallDetected)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //---------------------------------------------------------------------------------------

    private void Movement()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    #region LedgeClimbing

    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLedge)
        {
            //stop grabbing ledge once it's grabbed
            canGrabLedge = false;

            //fix roll bug on ledge climb
            rb.gravityScale = 0;

            //get location of ledge
            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            //set player position before and after climb
            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            //reset climb ability after climb
            canClimb = true;
        }

        //hold position of player and climbbegun position
        if (canClimb)
        {
            transform.position = climbBegunPosition;
        }
    }

    private void LedgeClimbOver()
    {
        //stop animation and allow player to continue, using event in animation
        canClimb = false;
        //fix roll bug on ledge
        rb.gravityScale = 5;

        transform.position = climbOverPosition;
        //fix double grab bug
        Invoke("AllowLedgeGrab", .1f);
    }

    private void AllowLedgeGrab() => canGrabLedge = true;

    #endregion

    private void AnimatorControllers()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("canClimb", canClimb);
    }

    private void CheckCollision()
    {
        //check if enemy is contacting ground
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        groundForward = Physics2D.Raycast(groundForwardCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        //check for ceiling when sliding
        ceilingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatIsGround);
        //check for wall interaction
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    //---------------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        //Line to check ground check distance variable
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundForwardCheck.position, new Vector2(groundForwardCheck.position.x, groundForwardCheck.position.y - groundCheckDistance));
        //Line to check ceiling distance
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceilingCheckDistance));
        //Set up a wall check
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
