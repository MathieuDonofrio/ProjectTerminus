using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Minimum distance this projectile will be visible")]
    public float minVisibleDistance = 1;

    [Tooltip("The renderer of the projectile")]
    public Renderer projectileRenderer;

    /* State */

    private GameObject shooter;

    private ProjectilePool pool;

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
        if(Physics.Linecast(lastPosition, transform.position,
            out RaycastHit hit, collisionLayer, QueryTriggerInteraction.Collide))
        {
            pool.SpawnBulletHole(hit.point, Quaternion.LookRotation(hit.normal));

            Kill();
        }

        lastPosition = transform.position;
    }

    /* Services */

    public void StartLaunch(GameObject shooter, ProjectilePool pool, LayerMask collisionLayer, float range, float speed)
    {
        this.shooter = shooter;
        this.pool = pool;
        this.collisionLayer = collisionLayer;
        this.range = range;
        this.speed = speed;

        distanceTraveled = 0;
        lastPosition = transform.position;
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
