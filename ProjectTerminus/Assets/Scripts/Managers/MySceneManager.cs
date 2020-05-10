using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : Singleton<MySceneManager>
{

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
        AsyncOperation ao = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        ao.completed += OnLoadOperationComplete;

    }

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
