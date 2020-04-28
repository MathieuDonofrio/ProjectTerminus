using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Determines how fast the health bar fill will interpolate between states")]
    public float sharpness = 6;

    [Tooltip("How long the change effect will last")]
    public float changeEffectDuration = 1.5f;

    [Tooltip("Health bar label")]
    public Text label;

    [Tooltip("Material for the health bar")]
    public Material healthBarMat;

    /* State */

    private float currentFill;

    private float targetFill;

    private float lastChange;

    private void Start()
    {
        healthBarMat.SetFloat("_Health", currentFill);
        healthBarMat.SetFloat("_Brightness", 0);
    }

    private void OnDestroy()
    {
        healthBarMat.SetFloat("_Health", 1);
        healthBarMat.SetFloat("_Brightness", 0);
    }

    private void LateUpdate()
    {
        if (currentFill != targetFill)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * sharpness);

            healthBarMat.SetFloat("_Health", currentFill);

            int percentage = Mathf.RoundToInt(targetFill * 100);

            label.text = percentage + "%";
        }

        float elapsed = Time.time - lastChange;

        if (elapsed <= changeEffectDuration)
        {
            float brightness = 1 - Mathf.Clamp01(elapsed / changeEffectDuration);

            healthBarMat.SetFloat("_Brightness", brightness);
        }
    }

    /* Services */

    public void UpdateFill(float fill)
    {
        if(targetFill != fill)
        {
            lastChange = Time.time;
        }

        targetFill = Mathf.Clamp01(fill);
    }
}
