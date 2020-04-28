using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class ZombieController : MonoBehaviour
{
    public Camera cam;
    public int rotationSpeed = 5;
    private NavMeshAgent agent;
    private bool walking = false;
    private bool attacking = false;
    private Animator zombieAnimator;
    private RaycastHit hit;
    

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Create a ray toward the mouse click position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);


            // We shoot the ray and we gather information about what we hit
            if(Physics.Raycast(ray, out hit))
            {
                // MOVE OUR AGENT
                agent.SetDestination(hit.point);

            }
        }

        // set the animations depending on our velocity

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            //Set the animations
            walking = true;
            attacking = false;
          
            // rotate the zombie toward the target
            var dirVector = hit.point - transform.position;
            transform.rotation = Quaternion.Slerp
            (transform.rotation, Quaternion.LookRotation(dirVector), rotationSpeed * Time.deltaTime);
        }
        else
        {
            attacking = true;
            walking = false;
        }

        zombieAnimator.SetBool("attack",attacking);
        zombieAnimator.SetBool("walking",walking);

    }
}
