using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Crosshair : MonoBehaviour
{
    /* Configuration */

    [Tooltip("This value multiplies the requested size")]
    public float expansionMultiplier = 10;

    [Tooltip("Determines how fast the crosshair expansion will interpolate between states")]
    public float sharpness = 2;

    /* State */

    private float currentSize;

    private float targetSize;

    /* Required Components */

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if(currentSize != targetSize)
        {
            if(targetSize > 1)
            {
                currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * sharpness);

                rectTransform.sizeDelta = new Vector2(currentSize, currentSize);

                if (transform.localScale != Vector3.one) transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = Vector3.zero;
            }

        }
    }

    /* Services */

    public void UpdateSize(float size)
    {
        targetSize = expansionMultiplier * size;
    }

}
