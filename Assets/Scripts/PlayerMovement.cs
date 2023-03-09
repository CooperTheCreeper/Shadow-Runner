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

    //set up slide mechanic
    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    private float slideCooldownCounter;
    private float slideTimeCounter;
    private bool isSliding;

    //check for collision with ground
    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceilingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool ceilingDetected;

    //Ledge grabbing
    [HideInInspector] public bool ledgeDetected;

    //Freeze player at ledgecheck position
    [Header("Ledge Info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLedge = true;
    private bool canClimb;

    //---------------------------------------------------------------------------------------

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

        //control for slide counter
        slideTimeCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        if (playerUnlocked)
            //set movement based off of public values
            Movement();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        CheckForLedge();

        CheckForSlide();

        CheckInput();
    }

    private void CheckForLedge()
    {
        if(ledgeDetected && canGrabLedge)
        {
            //stop grabbing ledge once it's grabbed
            canGrabLedge = false;

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
        transform.position = climbOverPosition;
        //fix double grab bug
        Invoke("AllowLedgeGrab", .1f);
    }

    private void AllowLedgeGrab() => canGrabLedge = true;

    private void CheckForSlide()
    {
        if (slideTimeCounter < 0 && !ceilingDetected)
            isSliding = false;
    }

    private void Movement()
    {
        if (wallDetected)
            return;

        if (isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);

        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    private void SlideButton()
    {
        if(rb.velocity.x != 0 && slideCooldownCounter < 0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    private void JumpButton()
    {
        if (isSliding)
        {
            return;
        }

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

    private void CheckInput()
    {
        if (Input.GetButtonDown("Right"))
            //set button for movement
            playerUnlocked = true;

        //Set button for jump
        if (Input.GetButtonDown("Jump"))
            JumpButton();

        //set up key for slide (change later to button down)
        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();

    }

    private void AnimatorControllers()
    {
        //control blend tree animation for jump/fall & idle/move
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);

        //control animations for double jump and slide
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);

        //control animations for ledge climb
        anim.SetBool("canClimb", canClimb);
    }

    private void CheckCollision()
    {
        //check if player is contacting ground
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        //check for ceiling when sliding
        ceilingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatIsGround);
        //check for wall interaction
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        //Line to check ground check distance variable
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        //Line to check ceiling distance
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceilingCheckDistance));
        //Set up a wall check
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
