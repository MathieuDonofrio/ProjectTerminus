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

    private float lastKickTime;

    private float lastKick;

    private float lastSpeed;

    private float sum;

    private bool centering;

    private void OnDestroy()
    {
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        
        if (!centering)
        {
            float delta = Time.time - lastKickTime;

            if (delta < lastSpeed)
            {
                sum = -lastKick * delta / lastSpeed;
            }
            else
            {
                Center();
            }
        }
        else
        {
            if(sum != 0)
            {
                float t = (Time.time - lastKickTime + lastSpeed) / lastSpeed;

                sum = Mathf.Lerp(sum, 0, t);
            }
        }

        if(transform.localPosition.z != sum)
        {
            transform.localPosition = new Vector3(0, 0, sum);
        }
    }

    /* Services */

    /// <summary>
    /// Applies gun kick to the gun kick system
    /// </summary>
    /// <param name="kick"></param>
    public void Kick(float kick, float speed)
    {
        lastKick = kick;

        lastSpeed = speed * 0.5f;

        centering = false;

        lastKickTime = Time.time;
    }

    /// <summary>
    /// Starts recentering the gun kick offset. Makes sum 0.
    /// </summary>
    public void Center()
    {
        centering = true;
    }

}
