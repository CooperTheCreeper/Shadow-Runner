
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //ref to rigidbody
    private Rigidbody2D rb;

    //access animator controller
    private Animator anim;

    //change player alpha for invinciblity indicator
    private SpriteRenderer sr;

    //Player Death
    private bool isDead;

    //bool for movement
    [HideInInspector] public bool playerUnlocked;

    //set up extra life visuals
    [HideInInspector] public bool extraLife;

    //Knockback
    [Header("Knockback Info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    //Speed Up and speed reset
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float defaultMilestoneIncrease;
    private float speedMilestone;

    //movement and jump
    [Header("Jump Info")]
    [SerializeField] private float jumpForce;

    //double jump
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;


    //set up slide mechanic
    [Header("Slide Info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    [HideInInspector] public float slideCooldownCounter;
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
    [SerializeField] private Vector2 offset1; //offset for position BEFORE climb
    [SerializeField] private Vector2 offset2; //offset for position AFTER climb

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
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncrease = milestoneIncreaser;

    }

    void Update()
    {
        CheckCollision();
        AnimatorControllers();

        //control for slide counter
        slideTimeCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        extraLife = moveSpeed >= maxSpeed;

        //Test Knockback until enemies enstated
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Knockback();
        //}

        ////Test Death until enemies enstated
        //if (Input.GetKeyDown(KeyCode.O) && !isDead)
        //{
        //    StartCoroutine(Die());
        //}

        //Death
        if (isDead)
        {
            return;
        }

        //Knockback stop command
        if (isKnocked)
        {
            return;
        }

        if (playerUnlocked)
            //set movement based off of public values
            SetUpMovement();

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        SpeedController();

        CheckForLedge();

        CheckForSlideCancel();

        CheckInput();
    }

    //---------------------------------------------------------------------------------------

    public void Damage()
    {
        //Knockback player and reset speed if going max speed for second chance
        if(extraLife)
        {
            Knockback();
            SpeedReset();
        }
        //otherwise, kill player
        else
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        AudioManager.instance.PlaySFX(3);
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        Time.timeScale = .6f;

        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector2(0, 0);
        GameManager.instance.GameEnded();
    }

    private IEnumerator Invincibility()
    {
        //player alpha blink
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

        canBeKnocked = false;
        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.3f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.3f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.4f);

        sr.color = originalColor;
        canBeKnocked = true;
    }

    #region Knockback

    private void Knockback()
    {
        //make player invincible after knockback
        if (!canBeKnocked)
        {
            return;
        }

        StartCoroutine(Invincibility());

        //knockback player
        isKnocked = true;
        rb.velocity = knockbackDir;
    }

    private void CancelKnockback() => isKnocked = false;

    #endregion

    #region SpeedControl

    private void SpeedReset()
    {
        //reset speed of player
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncrease;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
        {
            return;
        }

        //check for milestone reach
        if (transform.position.x > speedMilestone)
        {
            //change milestone
            speedMilestone = speedMilestone + milestoneIncreaser;

            //increase player speed
            moveSpeed = moveSpeed * speedMultiplier;

            //increase milestone distance
            milestoneIncreaser = milestoneIncreaser * speedMultiplier;

            //make sure movespeed isn't more than max speed
            if(moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }
        }
    }

    #endregion

    #region LedgeClimbing

    private void CheckForLedge()
    {
        if(ledgeDetected && canGrabLedge)
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

    private void CheckForSlideCancel()
    {
        if (slideTimeCounter < 0 && !ceilingDetected)
            isSliding = false;
    }

    private void SetUpMovement()
    {
        if (wallDetected)
        {
            SpeedReset();
            return;
        }

        if (isSliding)
        {
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
    }

    #region Inputs

    public void SlideButton()
    {
        if(rb.velocity.x != 0 && slideCooldownCounter < 0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    public void JumpButton()
    {
        if (isSliding)
        {
            return;
        }

        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AudioManager.instance.PlaySFX(Random.Range(1, 2));
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            AudioManager.instance.PlaySFX(Random.Range(1, 2));
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);       
        }
    }

    private void CheckInput()
    {
        //if (Input.GetButtonDown("Right"))
        //    //set button for movement
        //    playerUnlocked = true;

        //Set button for jump
        if (Input.GetButtonDown("Jump"))
            JumpButton();

        //set up key for slide (change later to button down)
        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();

    }

    #endregion

    #region Animations

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

        //control animations for knockback
        anim.SetBool("isKnocked", isKnocked);

        //control animations for roll
        if(rb.velocity.y < -20)
        {
            anim.SetBool("canRoll", true);
        }
    }

    private void RollAnimFinished() => anim.SetBool("canRoll", false);

    #endregion

    private void CheckCollision()
    {
        //check if player is contacting ground
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        //check for ceiling when sliding
        ceilingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatIsGround);
        //check for wall interaction
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
    }

    //---------------------------------------------------------------------------------------

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
