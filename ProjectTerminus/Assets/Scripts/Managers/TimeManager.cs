using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
    private const float normalFactor = 1f;

    private const float pausedFactor = 0f;

    public static void CancelEffect()
    {
        Time.timeScale = normalFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void PauseGame()
    {
        Time.timeScale = pausedFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
