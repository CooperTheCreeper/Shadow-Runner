using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null)
        {
            AudioManager.instance.PlaySFX(3);
            GameManager.instance.GameEnded();
            Time.timeScale = .01f;
        }
    }
}
