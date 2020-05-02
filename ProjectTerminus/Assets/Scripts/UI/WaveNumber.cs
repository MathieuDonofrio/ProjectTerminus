using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveNumber : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Amount of seconds for the transition in animation to compleat")]
    public float transitionInTime = 4;

    [Tooltip("Amount of seconds for the transition out animation to compleat ")]
    public float transitionOutTime = 2;

    [Tooltip("The text containing the wave number")]
    public Text number;

    [Tooltip("The material for the wave number")]
    public Material numberMat;

    /* State */

    private int targetWave;

    private int currentWave;

    private float lastWaveChange;

    private float lastSliderValue;

    private void Start()
    {
        currentWave = targetWave = 0;
        
        number.text = "0";
    }

    private void LateUpdate()
    {
        if (targetWave != currentWave) TransitionOut();
        if (targetWave == currentWave) TransitionIn();
    }

    private void OnDestroy()
    {
        numberMat.SetFloat("_Slider", 1);
    }

    private void TransitionOut()
    {
        float alpha = Mathf.Clamp01((Time.time - lastWaveChange) / transitionOutTime);

        number.color = new Color(1, 1, 1, 1 - alpha);

        if (alpha == 1)
        {
            currentWave = targetWave;

            UpdateText();

            lastWaveChange = Time.time;
        }
    }

    private void TransitionIn()
    {
        float sliderValue = Mathf.Clamp01((Time.time - lastWaveChange) / transitionInTime);

        if (sliderValue != lastSliderValue)
        {
            numberMat.SetFloat("_Slider", sliderValue);

            lastSliderValue = sliderValue;
        }
    }

    private void UpdateText()
    {
        number.text = currentWave.ToString();

        numberMat.SetFloat("_Slider", 0);

        number.color = new Color(1, 1, 1, 1);
    }

    /* Services */

    public void UpdateWave(int wave, bool skipTransitionOut)
    {
        if(wave != currentWave)
        {
            targetWave = wave;

            lastWaveChange = Time.time;

            if (skipTransitionOut)
            {
                currentWave = targetWave;

                UpdateText();
            }
        }
    }
}
