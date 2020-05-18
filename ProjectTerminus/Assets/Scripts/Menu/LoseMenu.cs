using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseMenu : MonoBehaviour
{
    private Button menuButton;
    public Text roundSurvivedText;

    void Start()
    {
        menuButton = GetComponentInChildren<Button>();
        menuButton.onClick.AddListener(MainMenu);
    }

    public void SetRoundSurvived(int roundSurvived)
    {
        roundSurvivedText.text = "ROUND SURVIVED: " + roundSurvived;
    }
   
    void MainMenu()
    {
        FindObjectOfType<MySceneManager>().LoadMainMenu();
    }
}
