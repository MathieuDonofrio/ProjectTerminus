using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class Healthbar : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Determines how fast the health bar fill will interpolate between states")]
    public float sharpness = 6;

    [Tooltip("Health bar label")]
    public Text label;

    /* Required Components */

    private Slider slider;

    /* State */

    private float currentFill;

    private float targetFill;

    private void Start()
    {
        slider = GetComponent<Slider>();

        if (slider.value != 1) slider.value = 1.0f;
    }


    private void LateUpdate()
    {
        if (currentFill != targetFill)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * sharpness);

            slider.value = currentFill;

            int percentage = Mathf.RoundToInt(targetFill * 100);

            label.text = percentage + "%";
        }
    }

    /* Services */

    public void UpdateFill(float fill)
    {
        targetFill = Mathf.Clamp01(fill);
    }
}
