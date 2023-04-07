using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //have enemy pick up coin
        if(collision.GetComponent<Enemy>() != null)
        {
            Destroy(gameObject);
        }

        //have player pick up coin
        if(collision.GetComponent<PlayerMovement>() != null)
        {
            AudioManager.instance.PlaySFX(0);
            GameManager.instance.coins++;
            Destroy(gameObject);
        }
    }
}
