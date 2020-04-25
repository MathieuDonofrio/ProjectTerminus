using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hitmarker : MonoBehaviour
{
    [Tooltip("Amount of time the hitmarket will last")]
    public float duration = 0.5f;

    [Header("Hitmarkers")]
    [Tooltip("Hitmarket displayed when shot has landed a hit but did not kill")]
    public GameObject hit;

    [Tooltip("Hitmarket displayed on a kill")]
    public GameObject kill;

    /* State */

    private float lastHit;

    private bool isLastHitKill;

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastHit < duration)
        {
            hit.SetActive(!isLastHitKill);
            kill.SetActive(isLastHitKill);
        }
        else
        {
            hit.SetActive(false);
            kill.SetActive(false);
        }
    }

    public void Hit(bool kill)
    {
        lastHit = Time.time;
        isLastHitKill = kill;
    }

}
