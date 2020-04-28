using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieController : MonoBehaviour
{
    public Camera cam;
    private NavMeshAgent agent;
    private int rotationSpeed = 10;
    private Animator basicZombieAnimator;
    private bool walking = false;
    private bool attacking = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        basicZombieAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Create a ray toward the mouse click position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            // We shoot the ray and we gather information about what we hit
            if(Physics.Raycast(ray, out hit))
            {
                // MOVE OUR AGENT
                agent.SetDestination(hit.point);

                //Make the character rotate
                var dirVector = hit.point - transform.position;
                transform.rotation = Quaternion.Slerp
                (transform.rotation, Quaternion.LookRotation(dirVector), rotationSpeed * Time.deltaTime);
            }

            //executing an action depending on if our player is moving
            if (!IsMoving())
            {
                //Set the animations
                walking = true;
                attacking = false;
                Debug.Log("Im moving");
            }
            else
            {
                walking = false;
                attacking = true;
                Debug.Log("Im supposed to be attacking");
            }

            basicZombieAnimator.SetBool("walking",walking);
            basicZombieAnimator.SetBool("attacking",attacking);

        }
    }

    public bool IsMoving()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }
}
