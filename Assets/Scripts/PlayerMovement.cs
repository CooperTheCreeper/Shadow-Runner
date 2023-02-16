using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //ref to rigidbody
    public Rigidbody2D rb;

    [Header ("Movement & Jumping")]
    //movement and jump
    public float moveSpeed;
    public float jumpForce;

    //bool for movement
    private bool runBegun;

    [Header("Collision info")]
    //check for collision with ground
    public float groundCheckDistance;
    public LayerMask whatIsGround;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (runBegun)
            //set movement based off of public values
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        CheckCollision();

        CheckInput();

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
            runBegun = true;

        //Set button for jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnDrawGizmos()
    {
        //Line to check ground check distance variable
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
