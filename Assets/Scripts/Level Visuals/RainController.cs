using DigitalRuby.RainMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    //reference the rainscript premade script scripty script
    private RainScript2D rainController => GetComponent<RainScript2D>();

    //Control rain intensity from the premade script
    [Range(0.0f, 1.0f)]
    [SerializeField] private float intensity;
    [SerializeField] private float targetIntensity;

    //Change rain intensity
    [SerializeField] private float changeRate = .05f;
    [SerializeField] private float minValue = .2f;
    [SerializeField] private float maxValue = .49f;

    //Randomize intensity
    [SerializeField] private float chanceToRain;
    [SerializeField] private float rainCheckCooldown;
    private float rainCheckTimer;

    private bool canChangeIntensity;

    //---------------------------------------------------------------------------------------

    private void Update()
    {
        //decrease timer constantly
        rainCheckTimer -= Time.deltaTime;

        //Assign value from premade script to this script
        rainController.RainIntensity = intensity;

        //change intesnity via button TEST
        if (Input.GetKeyDown(KeyCode.R))
        {
            canChangeIntensity = true;
        }

        CheckForRain();

        if (canChangeIntensity)
        {
            ChangeIntensity(); 
        }
    }

    //---------------------------------------------------------------------------------------

    private void CheckForRain()
    {
        //delay rain timer
        if(rainCheckTimer < 0)
        {
            rainCheckTimer = rainCheckCooldown;
            canChangeIntensity = true;

            //stop from going to max
            if (Random.Range(0, 100) < chanceToRain)
            {
                targetIntensity = Random.Range(minValue, maxValue);
            }
            else
            {
                targetIntensity = 0;
            }            
        }
    }

    private void ChangeIntensity()
    {
        //increase value
        if (intensity < targetIntensity)
        {
            intensity += changeRate * Time.deltaTime;

            //check if value reached
            if (intensity >= targetIntensity)
            {
                intensity = targetIntensity;
                canChangeIntensity = false;
            }
        }
        //decrease value
        if (intensity > targetIntensity)
        {
            intensity -= changeRate * Time.deltaTime;

            //check if value reached
            if (intensity <= targetIntensity)
            {
                intensity = targetIntensity;
                canChangeIntensity = false;
            }
        }
    }
}
