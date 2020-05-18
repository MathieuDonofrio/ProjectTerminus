using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeathEffectHandler : MonoBehaviour
{
    /* Configuration */

    [Tooltip("The zombie death effect particle system")]
    public ParticleSystem particleSystemPrefab;

    [Tooltip("The amount of time the effect lasts")]
    public float duration = 2;

    /* State */

    private Queue<ParticleSystem> pool;

    private Queue<KeyValuePair<ParticleSystem, float>> active;

    private void Start()
    {
        pool = new Queue<ParticleSystem>();

        active = new Queue<KeyValuePair<ParticleSystem, float>>();
    }

    private void FixedUpdate()
    {
        if(active.Count > 0 && Time.time - active.Peek().Value >= duration)
        {
            ParticleSystem system = active.Dequeue().Key;

            system.gameObject.SetActive(false);

            pool.Enqueue(system);
        }
    }

    /* Services */

    public void PlayDeathEffect(Vector3 position, Quaternion rotation)
    {
        ParticleSystem system;

        if(pool.Count > 0)
        {
            system = pool.Dequeue();

            system.transform.position = position;
            system.transform.rotation = rotation;

            system.gameObject.SetActive(true);
        }
        else
        {
            system = Instantiate(particleSystemPrefab, position, rotation, transform);
        }

        active.Enqueue(new KeyValuePair<ParticleSystem, float>(system, Time.time));

        system.Play();
    }
}
