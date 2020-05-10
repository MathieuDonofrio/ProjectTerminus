using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private MySceneManager mySceneManager;
    private WaveManager waveManager;

    void Start()
    {
        mySceneManager = GetComponent<MySceneManager>();
        waveManager = GetComponent<WaveManager>();
    }

    void Update()
    {
        if (waveManager.isGameOver)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        // delete all objects??
        foreach (var item in FindObjectsOfType<GameObject>())
        {
            Destroy(item.gameObject);
        }

        //return to main menu hopefully we will add the lose screen tomorrow
        mySceneManager.LoadMainMenu();
    }
}
