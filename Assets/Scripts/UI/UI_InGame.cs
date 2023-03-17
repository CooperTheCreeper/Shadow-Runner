using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    private PlayerMovement player;

    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI coinsText;

    [SerializeField] private Image heartEmpty;
    [SerializeField] private Image heartFull;

    private float distance;
    private int coins;



    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;

        InvokeRepeating("UpdateInfo", 0, .2f); 
    }

    private void UpdateInfo()
    {
        distance = GameManager.instance.distance;
        coins = GameManager.instance.coins;

        if(distance > 0)
        {
            distanceText.text = distance.ToString("#,#" + "  m");
        }

        if(coins > 0)
        {
            coinsText.text = GameManager.instance.coins.ToString("#,#");
        }

        heartEmpty.enabled = !player.extraLife;
        heartFull.enabled = player.extraLife;
        
    }
}
