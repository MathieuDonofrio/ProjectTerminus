using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Projectile prefab used for instanciation")]
    public Projectile projectilePrefab;

    /* State */

    private Queue<Projectile> projectiles;

    private void Start()
    {
        projectiles = new Queue<Projectile>();
    }

    /* Services */

    public void LaunchProjectile(GameObject shooter, Vector3 position, Quaternion rotation, float speed)
    {
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
            projectile = Instantiate(projectilePrefab, position, rotation);

            projectile.transform.parent = transform;
        }

        projectile.StartLaunch(shooter, this, speed);

        //Debug.DrawRay(projectile.transform.position, projectile.transform.forward, Color.red, 10f);

    }

    public void CheckIn(Projectile projectile)
    {
        projectile.FinishLaunch();

        projectile.gameObject.SetActive(false);

        projectiles.Enqueue(projectile);
    }
}
