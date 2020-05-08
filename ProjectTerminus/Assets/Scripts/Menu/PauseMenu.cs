using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : StateMachine<PauseMenu>
{
    public static bool GameIsPaused = false;
    public GameObject MyCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;


    protected override void Awake()
    {
        base.Awake();
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
        MyCanvas.SetActive(false);
        TimeManager.CancelEffect();
      //  FindObjectOfType<AudioManager>().Play("Theme");
        GameIsPaused = false;

    }

    void MainMenu()
    {
        
       // SetState(new EndGame());
        MyCanvas.SetActive(false);
        TimeManager.CancelEffect();
        foreach (GameObject gameObjects in FindObjectsOfType<GameObject>())
        {
            Destroy(gameObjects);
        }
    }
    void Pause()
    {
        MyCanvas.SetActive(true);
       // FindObjectOfType<AudioManager>().Stop("Theme");
        TimeManager.PauseGame();
        GameIsPaused = true;
    }

   
}
