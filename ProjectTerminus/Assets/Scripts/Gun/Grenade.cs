using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;
using UnityStandardAssets.Effects;

public class Grenade : MonoBehaviour
{


    public float delay = 3f;
    float countDown;
    public GameObject explosionEffect;
    bool hasExploded = false;
    public float radius = 5;
    public float force = 5;

    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countDown -= Time.deltaTime;

        if (countDown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position,radius);
            }


            ZombieController zombies = nearbyObject.GetComponent<ZombieController>();

            if (zombies != null)
            {
                zombies.ExplosionOnDeath(force,radius);
            }
        }
        Destroy(gameObject);
    }
}
