using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button levelButton;
    [SerializeField] private MySceneManager mySceneManager;

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        mySceneManager = FindObjectOfType<MySceneManager>();
    }

    void StartGame()
    {
        //use the scenemanager to start the game
        mySceneManager.LoadNextLevel();
    }

}
