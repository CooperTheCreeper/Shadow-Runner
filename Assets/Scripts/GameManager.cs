using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UI_Main ui;

    //lock unlock player with start
    public PlayerMovement player;

    public bool colorEntirePlatform;

    //Set up day night cycle
    [Header("Skybox Materials")]
    [SerializeField] private Material[] skyBoxMat;

    [Header("Purchased Color")]
    public Color platformColor;
    public Color playerColor;


    [Header("Score Info")]
    public int coins;
    public float distance;
    public float score;

    //---------------------------------------------------------------------------------------

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1;

        SetUpSkyBox(PlayerPrefs.GetInt("SkyBoxSetting"));

        //LoadColor(); //WHAT IS WRONG WITH YOU //Sort of working??? not really????
    }

    //---------------------------------------------------------------------------------------

    public void SetUpSkyBox(int i)
    {
        //choose day or night skybox
        if(i <= 1)
        {
            RenderSettings.skybox = skyBoxMat[i];
        }
        //randomize skybox
        else
        {
            RenderSettings.skybox = skyBoxMat[Random.Range(0, skyBoxMat.Length)];
        }
        //save choice for next run
        PlayerPrefs.SetInt("SkyBoxSetting", i);
        
    }

    public void SaveColor(float r, float g, float b)
    {
        PlayerPrefs.SetFloat("ColorR", r);
        PlayerPrefs.SetFloat("ColorG", g);
        PlayerPrefs.SetFloat("ColorB", b);
    }

    private void LoadColor()
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        Color newColor = new Color(PlayerPrefs.GetFloat("ColorR"),
                                   PlayerPrefs.GetFloat("ColorG"),
                                   PlayerPrefs.GetFloat("ColorB"),
                                   PlayerPrefs.GetFloat("ColorA", 1));

        sr.color = newColor;
    }

    //---------------------------------------------------------------------------------------

    private void Update()
    {
        if(player.transform.position.x > distance)
        {
            distance = player.transform.position.x;
        }
    }

    //---------------------------------------------------------------------------------------

    public void UnlockPlayer() => player.playerUnlocked = true;

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
      
    public void SaveInfo()
    {
        int savedCoins = PlayerPrefs.GetInt("Coins");

        PlayerPrefs.SetInt("Coins", savedCoins + coins);

        score = distance * coins;

        PlayerPrefs.SetFloat("LastScore", score);

        if(PlayerPrefs.GetFloat("HighScore") < score)
        {
            PlayerPrefs.SetFloat("HighScore", score);
        }
    }

    public void GameEnded()
    {
        SaveInfo();
        ui.OpenEndGameUI();
    }
}
