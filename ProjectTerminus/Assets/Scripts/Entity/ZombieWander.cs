using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ZombieAudioManager))]
[RequireComponent(typeof(Animator))]
public class ZombieWander : MonoBehaviour
{
    /* Configuration */

    public float switchTime = 5;

    /* Required Components */

    private NavMeshAgent agent;

    /* State */

    private float lastSwitch;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        GetComponent<Animator>().SetBool("walking", true);
        GetComponent<ZombieAudioManager>().PlayWalking();

        Switch();
    }

    private void FixedUpdate()
    {
        if(Time.time - lastSwitch > switchTime)
        {
            Switch();
        }
    }

    public void Switch()
    {
        Vector3 destination = FindOnNavMesh(transform.position, 12);

        agent.SetDestination(destination);

        lastSwitch = Time.time;
    }

    public Vector3 FindOnNavMesh(Vector3 point, float searchRadius)
    {
        point += Random.insideUnitSphere * searchRadius;

        if (NavMesh.SamplePosition(point, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return point;
    }
}
