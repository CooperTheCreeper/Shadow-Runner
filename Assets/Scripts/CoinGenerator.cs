using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    //Set amount of coins
    private int amountOfCoins;
    [SerializeField] private GameObject coinPrefab;

    //set min and max for random generation
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;

    // Start is called before the first frame update
    void Start()
    {
        //randomize number of coins generated
        amountOfCoins = Random.Range(minCoins, maxCoins);

        //change offset to center
        int additionalOffset = amountOfCoins / 2;

        for (int i = 0; i < amountOfCoins; i++)
        {
            //change spawn positions
            Vector3 offset = new Vector2(i - additionalOffset, 0);

            //spawn coins
            Instantiate(coinPrefab, transform.position + offset, Quaternion.identity, transform);
        }        
    }

}
