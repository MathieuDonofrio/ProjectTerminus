using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{

    /* Configuration */

    [Header("UI Elements")]
    [Tooltip("Health Bar UI element")]
    public Healthbar healthbar;

    [Tooltip("Crosshair UI element")]
    public Crosshair crosshair;

    [Tooltip("Hitmarker UI element")]
    public Hitmarker hitmarker;

    /* Services */

    public void UpdateHealth(float percentage)
    {
        healthbar.UpdateFill(percentage);
    }

    public void UpdateCrosshair(float size)
    {
        crosshair.UpdateSize(size);
    }

    public void Hitmarker(bool kill)
    {
        hitmarker.Hit(kill);
    }
}
