using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hitmarker : MonoBehaviour
{
    [Tooltip("Amount of time the hitmarket will last")]
    public float duration = 0.2f;

    [Tooltip("Starting size")]
    public float startSize = 100f;

    [Tooltip("Target size")]
    public float targetSize = 150f;

    [Header("Hitmarkers")]
    [Tooltip("Hitmarket displayed when shot has landed a hit but did not kill")]
    public RectTransform hit;

    [Tooltip("Hitmarket displayed on a kill")]
    public RectTransform kill;

    /* State */

    private float lastHit;

    private bool isLastHitKill;

    private float size;

    private void Start()
    {
        lastHit = float.MinValue;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Time.time - lastHit < duration)
        {
            size = Mathf.Lerp(size, targetSize, 2 * Time.deltaTime / duration);

            hit.gameObject.SetActive(!isLastHitKill);
            kill.gameObject.SetActive(isLastHitKill);

            Vector2 scale2d = new Vector2(size, size);

            if (isLastHitKill)
            {
                kill.sizeDelta = scale2d;
            }
            else
            {
                hit.sizeDelta = scale2d;
            }
        }
        else
        {
            hit.gameObject.SetActive(false);
            kill.gameObject.SetActive(false);
        }
    }

    public void Hit(bool kill)
    {
        size = startSize;

        isLastHitKill = kill;

        lastHit = Time.time;
    }

}
