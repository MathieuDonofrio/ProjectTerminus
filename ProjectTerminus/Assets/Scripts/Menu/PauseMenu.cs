using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject MyPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;


    protected void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        menuButton.onClick.AddListener(MainMenu);
        quitButton.onClick.AddListener(Quit);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
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

    void Quit()
    {
        Application.Quit();
    }

    void ResumeGame()
    {
        MyPanel.SetActive(false);
        TimeManager.CancelEffect();
      //  FindObjectOfType<AudioManager>().Play("Theme");
        GameIsPaused = false;

    }

    void MainMenu()
    {
        //end the game
        FindObjectOfType<GameManager>().EndGame();

    }
    void Pause()
    {
        MyPanel.SetActive(true);
       // FindObjectOfType<AudioManager>().Stop("Theme");
        TimeManager.PauseGame();
        GameIsPaused = true;
    }

   
}
