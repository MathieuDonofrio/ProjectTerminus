using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BloodOverlay : MonoBehaviour
{

    /* Configuration */

    [Tooltip("Determines how fast the health bar fill will interpolate between states")]
    public float sharpness = 12;

    /* Required Components */

    private Image blood;

    /* State */

    private float currentFill;

    private float targetFill;

    private void Start()
    {
        blood = GetComponent<Image>();
    }

    private void LateUpdate()
    {
        if(currentFill != targetFill)
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * sharpness);

            blood.color = new Color(1, 1, 1, currentFill);
        }
    }

    public void UpdateFill(float fill)
    {
        targetFill = Mathf.Clamp01(fill);
    }

}
