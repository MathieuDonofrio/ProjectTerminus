using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Projectile : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Minimum distance this projectile will be visible")]
    public float minVisibleDistance = 0.5f;

    [Tooltip("The renderer of the projectile")]
    public Renderer projectileRenderer;

    /* State */

    private GameObject shooter;

    private ProjectileHandler pool;

    private LayerMask collisionLayer;

    private float range;

    private float speed;

    private Vector3 lastPosition;

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

        if (distanceTraveled >= range) Kill();
    }

    private void FixedUpdate()
    {

#if UNITY_EDITOR
        Profiler.BeginSample("ProjectileFixedUpdate");
#endif

        if (Physics.Linecast(lastPosition, transform.position,
            out RaycastHit hit, collisionLayer, QueryTriggerInteraction.Collide))
        {
            // TODO damage entity

            pool.SpawnBulletHole(hit.point, Quaternion.LookRotation(hit.normal));

            Kill();
        }

        lastPosition = transform.position;

#if UNITY_EDITOR
        Profiler.EndSample();
#endif

    }

    /* Services */

    public void StartLaunch(GameObject shooter, ProjectileHandler pool, Vector3 rayStart, LayerMask collisionLayer, float range, float speed)
    {
        this.shooter = shooter;
        this.pool = pool;
        this.collisionLayer = collisionLayer;
        this.range = range;
        this.speed = speed;

        lastPosition = rayStart;
        distanceTraveled = 0;
        projectileRenderer.enabled = false;
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
