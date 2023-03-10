using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    //Check position of ledge check
    [SerializeField] private float radius;

    [SerializeField] private LayerMask whatIsGround;

    //reference the playermovement script
    [SerializeField] public PlayerMovement player;

    //On trigger enter for detection
    private bool canDetected;

    private void Update()
    {
        if (canDetected)
        {
            player.ledgeDetected = Physics2D.OverlapCircle(transform.position, radius, whatIsGround);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetected = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canDetected = true;
        }
    }

    private void OnDrawGizmos()
    {
        //Visual for ledge check positioning
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
