using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    //access sprite renderer
    [SerializeField] private SpriteRenderer sr;

    //ref header for platform
    [SerializeField] private SpriteRenderer headerSr;

    private void Start()
    {
        //assign header parent so it doesn't look wonky on resizes
        headerSr.transform.parent = transform.parent;
        headerSr.transform.localScale = new Vector2(sr.bounds.size.x, .2f);
        headerSr.transform.position = new Vector2(transform.position.x, sr.bounds.max.y - .1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerMovement>() != null)
        {
            if (GameManager.instance.colorEntirePlatform)
            {
                //headerSr.color = GameManager.instance.platformColor;
                sr.color = GameManager.instance.platformColor;
            }
            else
            {
                headerSr.color = GameManager.instance.platformColor;
            }
        }
    }


}
