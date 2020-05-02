using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyParticleSystem : MonoBehaviour
{
    /* Configuration */

    [Tooltip("The money particle prefab to instanciate")]
    public MoneyParticle particlePrefab;

    /* State */

    private Queue<MoneyParticle> pool = new Queue<MoneyParticle>();

    /* Services */

    public void SpawnParticle(Vector2 position, int amount)
    {
        MoneyParticle particle;

        position += Vector2.right * 60;

        if (pool.Count > 0)
        {
            particle = pool.Dequeue();

            particle.transform.position = position;

            particle.gameObject.SetActive(true);
        }
        else
        {
            particle = Instantiate(particlePrefab, position, Quaternion.identity, transform);
        }

        particle.StartParticle(this, amount);
    }

    public void CheckIn(MoneyParticle particle) 
    {
        particle.gameObject.SetActive(false);

        pool.Enqueue(particle);
    }
}
