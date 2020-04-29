using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Maximum distance this projectile can travel")]
    public float maxDistance = 100;

    [Tooltip("Minimum distance this projectile will be visible")]
    public float minVisibleDistance = 1;

    [Tooltip("The renderer of the projectile")]
    public Renderer projectileRenderer;

    /* State */

    private GameObject shooter;

    private ProjectilePool pool;

    private float speed;

    private float distanceTraveled;

    private void Update()
    {
        Vector3 movement = transform.forward * Time.deltaTime * speed;

        transform.position += movement;

        distanceTraveled += movement.magnitude;

        if (!projectileRenderer.enabled && distanceTraveled >= minVisibleDistance)
        {
            projectileRenderer.enabled = true;
        }

        if (distanceTraveled >= maxDistance)
        {
            if(pool == null)
            {
                Destroy(gameObject);
            }
            else
            {
                pool.CheckIn(this);
            }
        }
    }

    /* Services */

    public void StartLaunch(GameObject shooter, ProjectilePool pool, float speed)
    {
        this.shooter = shooter;
        this.pool = pool;
        this.speed = speed;

        distanceTraveled = 0;
        projectileRenderer.enabled = false;
    }

    public void FinishLaunch()
    {
        projectileRenderer.enabled = false;
    }

}
