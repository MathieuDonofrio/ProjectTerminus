using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunKickSystem : MonoBehaviour
{    
    /* Configuration */

    [Tooltip("The gun holder to get current gun from")]
    public GunHolder gunHolder;

    [Tooltip("The maximum amount of offset")]
    public float maxOffset = 0.1f;

    /* State */

    [Header("Debug")]

    public float sum;

    public float add;

    public bool centering;

    private void OnDestroy()
    {
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        if (add != 0)
        {
            float movement = add < 0.01f ? add : add * Time.deltaTime * 2;

            add -= movement;
            sum += movement;
        }
        else if (sum != 0)
        {
            Center();
        }

        if(transform.localPosition.z != sum)
        {
            transform.localPosition = new Vector3(0, 0, Mathf.Min(sum, maxOffset));
        }
    }

    /* Services */

    /// <summary>
    /// Applies gun kick to the gun kick system
    /// </summary>
    /// <param name="kick"></param>
    public void Kick(float kick)
    {
        if (centering)
        {
            add = kick;
        }
        else
        {
            add += kick;
        }

        centering = false;
    }

    /// <summary>
    /// Starts recentering the gun kick offset. Makes sum 0.
    /// </summary>
    public void Center()
    {
        add = -sum;

        centering = true;
    }

}
