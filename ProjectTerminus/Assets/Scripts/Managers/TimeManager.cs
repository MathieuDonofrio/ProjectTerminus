using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private static readonly float slowdownFactor = .05f;
    private static readonly float normalFactor = 1f;
    private static readonly float PausedFactor = 0f;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public static void DoSlowMotion()
    {
        Time.timeScale = slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public static void CancelEffect()
    {
        Time.timeScale = normalFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public static void PauseGame()
    {
        Time.timeScale = PausedFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
