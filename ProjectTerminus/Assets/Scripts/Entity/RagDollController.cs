using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollController : MonoBehaviour
{

    public Collider[] AllColliders;

    private void Awake()
    {
        AllColliders = GetComponentsInChildren<Collider>(true);
        ActivateRagdoll(false);
    }

    public void ActivateRagdoll(bool active)
    {
        foreach (var col in AllColliders)
            col.enabled = active;

        GetComponent<Rigidbody>().useGravity = !active;
        GetComponent<Animator>().enabled = !active;
    }
    
}
