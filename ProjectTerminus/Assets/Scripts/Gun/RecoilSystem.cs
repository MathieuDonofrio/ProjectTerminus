using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilSystem : MonoBehaviour
{
    /* Configuration */

    [Tooltip("The gun holder to get current gun from")]
    public GunHolder gunHolder;

    /* State */

    private Vector2 sum;

    private Vector2 add;

    private float lastKickSpeed;

    private float lastCenterSpeed;

    private bool centering;

    private void OnDestroy()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    private void Update()
    {
        if(add != Vector2.zero)
        {
            float speed = centering ? lastCenterSpeed : lastKickSpeed;

            Vector2 movement = add.magnitude < 0.01f ? add : add * Time.deltaTime / speed;

            add -= movement;
            sum += movement;
        }
        else if(sum != Vector2.zero)
        {
            Center();
        }

        transform.localEulerAngles = new Vector3(-sum.y, sum.x, 0);
    }

    /* Services */

    /// <summary>
    /// Applies recoil to the recoil system
    /// </summary>
    /// <param name="kick"></param>
    public void Kick(Vector2 kick)
    {
        if (centering)
        {
            add = kick;
        }
        else
        {
            add += kick;
        }

        lastKickSpeed = GetKickSpeed();

        centering = false;
    }

    /// <summary>
    /// Starts recentering the recoil offset. Makes sum 0.
    /// </summary>
    public void Center()
    {
        add = -sum;

        lastCenterSpeed = GetCenterSpeed();

        centering = true;
    }

    /// <summary>
    /// Returns the kick speed for the currently held gun,
    /// or the default if the gun is null.
    /// </summary>
    /// <returns>kick speed</returns>
    public float GetKickSpeed()
    {
        GunController gun = gunHolder.CurrentHeldGun();

        return gun == null ? 0.01f : gun.kickSpeed;
    }

    /// <summary>
    /// Returns the center speed for the currently held gun,
    /// or the default if the gun is null
    /// </summary>
    /// <returns>center speed</returns>
    public float GetCenterSpeed()
    {
        GunController gun = gunHolder.CurrentHeldGun();

        return gun == null ? 0.25f : gun.centerSpeed;
    }

}
