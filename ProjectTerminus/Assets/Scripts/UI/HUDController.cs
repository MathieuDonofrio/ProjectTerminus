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

    /// <summary>
    /// Updates the health bar and the blood overlay for the specified percentage of health.
    /// </summary>
    /// <param name="percentage">health percentage between 0 and 1</param>
    public void UpdateHealth(float percentage)
    {
        // Update health bar
        healthbar.UpdateFill(percentage);

        // TODO update blood overlay
    }

    /// <summary>
    /// Updates the crosshair's size for the gun accuracy and the players movement.
    /// </summary>
    /// <param name="accuracy">the held gun accuracy</param>
    /// <param name="movement">the player velocity magnitude</param>
    /// <param name="hide">if the crosshair needs to be hidden</param>
    public void UpdateCrosshair(float accuracy, float movement, CrosshairType type = CrosshairType.DEFAULT)
    {
        // Get size
        float size = (1 + accuracy) * (1 + Mathf.Abs(movement));

        // Update crosshair size
        crosshair.UpdateSize(size);

        // Update type
        crosshair.UpdateType(type);
    }

    /// <summary>
    /// Flags a hitmarket to be displayed for a period of time.
    /// </summary>
    /// <param name="kill">if the hit resulted in a kill</param>
    public void Hitmarker(bool kill = false)
    {
        // Flag hit
        hitmarker.Hit(kill);
    }
}
