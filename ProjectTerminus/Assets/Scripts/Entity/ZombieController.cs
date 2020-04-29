using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent;
    private int rotationSpeed = 10;
    private Animator zombieAnimator;
    private bool walking = false;
    private bool attacking = false;
    public Transform playerTransform;
    private bool dead = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dead = !dead;
        }

        if (!dead)
        {

            // MOVE OUR AGENT
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);


            //executing an action depending on if our player is moving
            if (IsWithinStoppingDistance())
            {
                //Set the animations
                walking = true;
                attacking = false;

            }
            else
            {
                walking = false;
                attacking = true;
                Debug.Log("Im supposed to be attacking");
            }
            zombieAnimator.SetBool("walking", walking);
            zombieAnimator.SetBool("attacking", attacking);

            //Make the character rotate
            var dirVector = playerTransform.position - transform.position;
            transform.rotation = Quaternion.Slerp
            (transform.rotation, Quaternion.LookRotation(dirVector), rotationSpeed * Time.deltaTime);
        }
        else
        {
            agent.isStopped = true;
            walking = false;
            attacking = false;
            zombieAnimator.SetBool("walking", walking);
            zombieAnimator.SetBool("attacking", attacking);
            zombieAnimator.SetBool("dying", dead);
        }

    }

    public bool IsWithinStoppingDistance()
    {
        return !(Vector3.Distance(transform.position, playerTransform.position) < agent.stoppingDistance);
    }
}
