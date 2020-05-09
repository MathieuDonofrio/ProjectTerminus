using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagDollController : MonoBehaviour
{

    private Collider mainCollider;

    private Collider[] allColliders;

    private void Awake()
    {
        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>();

        ActivateRagdoll(false);
    }

    public void ExplosionOnDeath(float force, float radius)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        rb.isKinematic = false;

        // rb.AddExplosionForce(force, transform.position, radius);

        foreach (Rigidbody rigidbody in rbs)
        {
            rigidbody.AddExplosionForce(force, transform.position, radius);
        }

    }

    public void ActivateRagdoll(bool active)
    {
        foreach (var col in allColliders)
            col.enabled = active;

        mainCollider.enabled = !active;

        GetComponent<NavMeshAgent>().enabled = !active;
        GetComponent<Animator>().enabled = !active;
    }
    
}
