using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveNumber : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Amount of seconds for the wave change animation to take")]
    public float animationTime = 4;

    [Tooltip("The text containing the wave number")]
    public Text number;

    [Tooltip("The material containing the wave number shader")]
    public Material material;

    /* State */

    private int targetWave;

    private int currentWave;

    private float lastWaveChange;

    private float lastSliderValue;

    private void Start()
    {
        currentWave = 0;
        number.text = "0";
    }

    private void Update()
    {
        if(targetWave != currentWave)
        {
            currentWave = targetWave;

            number.text = currentWave.ToString();
        }

        float sliderValue = Mathf.Clamp01((Time.time - lastWaveChange) / animationTime);

        if(sliderValue != lastSliderValue)
        {
            material.SetFloat("_Slider", sliderValue);

            lastSliderValue = sliderValue;
        }

    }

    /* Services */

    public void UpdateWave(int wave)
    {
        if(wave != currentWave)
        {
            number.text = string.Empty;

            targetWave = wave;

            lastWaveChange = Time.time;
        }
    }
}
