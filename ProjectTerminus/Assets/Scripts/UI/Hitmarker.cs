using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hitmarker : MonoBehaviour
{
    [Tooltip("Amount of time the hitmarket will last")]
    public float duration = 0.02f;

    [Header("Hitmarkers")]
    [Tooltip("Hitmarket displayed when shot has landed a hit but did not kill")]
    public RectTransform hit;

    [Tooltip("Hitmarket displayed on a kill")]
    public RectTransform kill;

    /* State */

    private float lastHit;

    private bool isLastHitKill;

    private void Start()
    {
        lastHit = float.MinValue;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Time.time - lastHit < duration)
        {
            hit.gameObject.SetActive(!isLastHitKill);
            kill.gameObject.SetActive(isLastHitKill);
        }
        else
        {
            hit.gameObject.SetActive(false);
            kill.gameObject.SetActive(false);
        }
    }

    public void Hit(bool kill)
    {
        lastHit = Time.time;
        isLastHitKill = kill;
    }

}
