using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : Singleton<MySceneManager>
{

    public GameObject loadingScreen;
    public Text progressText;
    public Slider slider;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMainMenu()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(0);
        ao.completed += OnLoadOperationComplete;
    }

    public void LoadNextLevel()
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            //make  0 to .9 go 0 to 1
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }
    }

    //YAGNI
    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        //wait until the asynchronous scene is fully loaded
        while (!ao.isDone)
        {
            Debug.Log("waiting to load level " + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
    }

    private void OnLoadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Level completed");
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.Log("Unable to unload level " + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }

    public void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload completed");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
