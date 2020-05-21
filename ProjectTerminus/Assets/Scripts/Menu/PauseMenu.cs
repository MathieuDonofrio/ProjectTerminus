using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject MyPanel;

    public GameObject hud;

    public Button resumeButton;

    public Button menuButton;

    public Button quitButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        menuButton.onClick.AddListener(MainMenu);
        quitButton.onClick.AddListener(Quit);

        TimeManager.CancelEffect();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                Pause();
            }
        }
    }

    /* Services */

    public void ResumeGame()
    {
        MyPanel.SetActive(false);

        hud.SetActive(true);

        TimeManager.CancelEffect();

        GameIsPaused = false;
    }

    public void Pause()
    {
        MyPanel.SetActive(true);

        hud.SetActive(false);

        TimeManager.PauseGame();

        GameIsPaused = true;
    }

    public void MainMenu()
    {
        FindObjectOfType<MySceneManager>().LoadMainMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
