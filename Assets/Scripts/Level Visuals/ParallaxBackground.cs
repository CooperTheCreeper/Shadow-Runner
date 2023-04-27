using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    //position of the camera
    private GameObject cam;

    //parallax effect
    [SerializeField] private float parallaxEffect;

    //length of BG
    private float length;
    private float xPosition;


    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        //know how far BG has moved
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);

        //define the distance to move BG
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        //change position of BG with distance
        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if(distanceMoved > xPosition + length)
        {
            xPosition = xPosition + length;
        }
    }
}
