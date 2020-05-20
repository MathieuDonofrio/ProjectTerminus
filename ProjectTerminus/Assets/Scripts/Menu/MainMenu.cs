using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;

    public Button levelButton;

    private MySceneManager mySceneManager;

    private void Start()
    {
        mySceneManager = FindObjectOfType<MySceneManager>();

        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        mySceneManager.LoadNextLevel();
    }

}
