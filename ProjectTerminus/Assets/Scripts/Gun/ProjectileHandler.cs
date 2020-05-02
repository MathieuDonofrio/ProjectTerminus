using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Projectile prefab used for instanciation")]
    public Projectile projectilePrefab;

    [Tooltip("The bullethole prefab used for instanciation")]
    public BulletHole bulletHolePrefab;

    [Tooltip("The particle system used for muzzle flash effects")]
    public ParticleSystem muzzleFlashParticleSystem;

    [Tooltip("The amount of projectiles to pre instanciate")]
    public float preInstanciatedProjectiles = 8;

    [Tooltip("The amount of bullet holes to pre instanciate")]
    public float preInstanciatedBulletHoles = 18;

    [Tooltip("The amount of time a bullet hole stays active")]
    public float bulletHoleDuration = 4;

    /* State */

    private Queue<Projectile> projectiles;

    private Queue<BulletHole> bulletHoles;

    private void Start()
    {
        if(projectilePrefab != null)
        {
            projectiles = new Queue<Projectile>();

            for (int i = 0; i < preInstanciatedProjectiles; i++)
            {
                CheckIn(Instantiate(projectilePrefab, transform));
            }

        }

        if (bulletHolePrefab != null)
        {
            bulletHoles = new Queue<BulletHole>();

            for (int i = 0; i < preInstanciatedBulletHoles; i++)
            {
                CheckIn(Instantiate(bulletHolePrefab, transform));
            }
        }
    } 

    /* Services */

    public void LaunchProjectile(GameObject shooter, Vector3 rayStart, Vector3 position, Quaternion rotation, float range, float speed)
    {
        if (projectilePrefab == null)
            return;

        Projectile projectile;

        if(projectiles.Count > 0)
        {
            projectile = projectiles.Dequeue();

            projectile.transform.position = position;
            projectile.transform.rotation = rotation;

            projectile.gameObject.SetActive(true);
        }
        else
        {
            projectile = Instantiate(projectilePrefab, position, rotation, transform);
        }

        projectile.StartLaunch(shooter, this, rayStart, -1, range, speed);
    }

    public void SpawnBulletHole(Vector3 position, Quaternion rotation)
    {
        if (bulletHolePrefab == null)
            return;

        BulletHole bulletHole;

        if(bulletHoles.Count > 0)
        {
            bulletHole = bulletHoles.Dequeue();

            bulletHole.transform.position = position;
            bulletHole.transform.rotation = rotation;

            bulletHole.gameObject.SetActive(true);
        }
        else
        {
            bulletHole = Instantiate(bulletHolePrefab, position, rotation, transform);
        }

        bulletHole.Spawn(this);
    }

    public void SpawnMuzzleFlash(Vector3 position, Vector3 rotation)
    {
        var emitParams = new ParticleSystem.EmitParams();

        emitParams.position = position;
        emitParams.rotation3D = rotation + new Vector3(90, 0, 0);

        muzzleFlashParticleSystem.Emit(emitParams, 1);
    }

    public void CheckIn(Projectile projectile)
    {
        if (projectilePrefab == null)
            return;

        projectile.gameObject.SetActive(false);

        projectiles.Enqueue(projectile);
    }

    public void CheckIn(BulletHole bulletHole)
    {
        if (bulletHolePrefab == null)
            return;

        bulletHole.gameObject.SetActive(false);

        bulletHoles.Enqueue(bulletHole);
    }

}
