using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    /* Configuration */

    [Tooltip("This value multiplies the requested size")]
    public float expansionMultiplier = 10;

    [Tooltip("Determines how fast the crosshair expansion will interpolate between states")]
    public float sharpness = 2;

    [Header("Crosshairs")]
    [Tooltip("Default crosshair applied when not aiming")]
    public RectTransform crosshair;

    [Tooltip("Crosshair applied aiming in a reflex sight")]
    public RectTransform ironSight;

    [Tooltip("Crosshair applied aiming in a reflex sight")]
    public RectTransform reflexSight;

    /* State */

    private float currentSize;

    private float targetSize;

    private CrosshairType type;

    private void Start()
    {
        type = CrosshairType.DEFAULT;

        crosshair.gameObject.SetActive(true);
        reflexSight.gameObject.SetActive(false);
        ironSight.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if(type == CrosshairType.DEFAULT && currentSize != targetSize)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * sharpness);

            Current().sizeDelta = new Vector2(currentSize, currentSize);
        }
    }

    private RectTransform Current()
    {
        if (type == CrosshairType.IRON_SIGHT) return ironSight;
        else if (type == CrosshairType.REFLEX_SIGHT) return reflexSight;
        else return crosshair;
    }

    /* Services */

    public void UpdateType(CrosshairType type)
    {
        if (this.type == type)
            return;

        Current().gameObject.SetActive(false);

        this.type = type;

        Current().gameObject.SetActive(true);
    }

    public void UpdateSize(float size)
    {
        targetSize = expansionMultiplier * size;
    }

}

public enum CrosshairType
{
    DEFAULT, IRON_SIGHT, REFLEX_SIGHT
}
