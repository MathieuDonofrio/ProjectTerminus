using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseMenu : MonoBehaviour
{
    public GameObject hud;

    public GameObject losePanel; 

    public Text roundSurvivedText;

    public Text highscoreText;

    public Button menuButton;

    private void Start()
    {
        menuButton.onClick.AddListener(MainMenu);
    }

    public void MainMenu()
    {
        FindObjectOfType<MySceneManager>().LoadMainMenu();
    }

    public void LostGame(int roundSurvived)
    {
        roundSurvivedText.text = "ROUNDS SURVIVED: " + roundSurvived;

        int highscore = PlayerPrefs.GetInt("highscore");

        if (highscore < roundSurvived) highscore = roundSurvived;

        PlayerPrefs.SetInt("highscore", highscore);

        highscoreText.text = "HIGHSCORE: " + highscore;

        losePanel.SetActive(true);

        Invoke("StopGame", 2.0f);
    }

    public void StopGame()
    {
        hud.SetActive(false);

        TimeManager.PauseGame();
    }
  
}
