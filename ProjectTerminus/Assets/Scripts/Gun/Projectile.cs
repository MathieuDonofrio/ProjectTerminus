using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Minimum distance this projectile will be visible")]
    public float minVisibleDistance = 0.5f;

    [Tooltip("The renderer of the projectile")]
    public Renderer projectileRenderer;

    /* State */

    private GunController shooter;

    private ProjectileHandler pool;

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
            out RaycastHit hit, -1, QueryTriggerInteraction.Collide))
        {
            bool landedHit = false;

            Entity entity = hit.collider.GetComponent<Entity>();

            if(entity != null)
            {
                if (!entity.IsDead && entity.gameObject != shooter.gunHolder.gameObject)
                {
                    float damage = shooter.damage + Random.value * shooter.randomDamage;

                    bool headshot = Mathf.Abs(entity.transform.position.y + entity.eyeHeight - hit.point.y) <= entity.headSize;

                    if (headshot) damage *= shooter.headshotModifier;

                    bool kill = entity.Damage(damage, shooter.gameObject, DamageType.PROJECTILE);

                    if (shooter.gunHolder != null)
                    {
                        shooter.gunHolder.LandedHit(kill, headshot);
                    }

                    DoImpact(entity);

                    pool.SpawnBloodSplat(hit.point, 2 + Mathf.RoundToInt(2 * Random.value));

                    landedHit = true;
                }
            }
            else
            {
                pool.SpawnBulletHole(hit.point, Quaternion.LookRotation(hit.normal));

                landedHit = true;
            }

            if (landedHit)
            {
                Kill();
            }
        }

        lastPosition = transform.position;

#if UNITY_EDITOR
        Profiler.EndSample();
#endif

    }

    private void DoImpact(Entity entity)
    {
        NavMeshAgent agent = entity.GetComponent<NavMeshAgent>();

        if(agent != null)
        {            
            agent.velocity = transform.forward * shooter.impactStrength;
        }
    }

    /* Services */

    public void StartLaunch(GunController shooter, ProjectileHandler pool, Vector3 rayStart, float range, float speed)
    {
        this.shooter = shooter;
        this.pool = pool;
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
