using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerMovement player;

    //move the RB
    [Header("Movement Details")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float distanceToRun;
    private float maxDistance;

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

    //animations for ground pound thingy
    private bool justRespawned = true;

    //increase floaty-ness
    private float defaultGravityScale;

    //---------------------------------------------------------------------------------------

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameManager.instance.player;

        rb.velocity = new Vector2(10, 15);
        defaultGravityScale = rb.gravityScale;

        rb.gravityScale = rb.gravityScale * .6f;

        maxDistance = transform.position.x + distanceToRun;
    }

    private void Update()
    {

        if(justRespawned)
        {
            if(rb.velocity.y < 0)
            {
                rb.gravityScale = defaultGravityScale * 2;
            }

            if (isGrounded)
            {
                rb.velocity = new Vector2(0, 0);
            }

        }

        CheckCollision();
        AnimatorControllers();
        Movement();
        CheckForLedge();
        SpeedController();

        if(transform.position.x > maxDistance)
        {
            canMove = false;
            return;
        }

        //have enemy jump if there is no ground on the check
        if(!groundForward || wallDetected)
        {
            Jump();
        }
    }

    //---------------------------------------------------------------------------------------

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void SpeedController()
    {
        bool playerAhead = player.transform.position.x > transform.position.x;
        bool playerFarAway = Vector2.Distance(player.transform.position, transform.position) > 2.5f;

        if (playerAhead)
        {
            if (playerFarAway)
            {
                moveSpeed = 25;
            }
            else
            {
                moveSpeed = 17;
            }
        }
        else
        {
            if (playerFarAway)
            {
                moveSpeed = 11;
            }
            else
            {
                moveSpeed = 14;
            }
        }
    }

    private void Movement()
    {
        if (justRespawned)
        {
            return;
        }

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
        anim.SetBool("justRespawned", justRespawned);
    }

    private void AnimationTrigger()
    {
        rb.gravityScale = defaultGravityScale;
        justRespawned = false;
        canMove = true;
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
