using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagDollController : MonoBehaviour
{

    private Collider mainCollider;

    private Collider[] allColliders;
    private Rigidbody[] allRigidBodies;

    private void Awake()
    {
        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>();
        allRigidBodies = GetComponentsInChildren<Rigidbody>();
        DeactivateColliders();
    }

    public void ExplosionOnDeath(float force, float radius)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
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

        foreach (var rig in allRigidBodies)
        {
            rig.isKinematic = false;
            rig.useGravity = true;
        }

        mainCollider.enabled = !active;

        GetComponent<NavMeshAgent>().enabled = !active;
        GetComponent<Animator>().enabled = !active;
    }

    private void DeactivateColliders()
    {
        foreach (var rig in allRigidBodies)
        {
            rig.isKinematic = true;
            rig.useGravity = false;
        }
    }
    
}
