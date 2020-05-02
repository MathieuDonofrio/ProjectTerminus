using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHole : MonoBehaviour
{

    /* State */

    private ProjectileHandler pool;

    private float startTime;

    public void FixedUpdate()
    {
        if (Time.time - startTime >= pool.bulletHoleDuration) Kill();
    }

    /* Services */

    public void Spawn(ProjectileHandler pool)
    {
        this.pool = pool;

        startTime = Time.time;
    }

    public void Kill()
    {
        if (pool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            pool.CheckIn(this);
        }
    }
}
