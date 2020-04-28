using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent;
    private int rotationSpeed = 10;
    private Animator basicZombieAnimator;
    private bool walking = false;
    private bool attacking = false;
    public Transform playerTransform;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        basicZombieAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        // MOVE OUR AGENT
        agent.SetDestination(playerTransform.position);


        //executing an action depending on if our player is moving
        if (IsMoving())
        {
            //Set the animations
            walking = true;
            attacking = false;

            //Make the character rotate
            var dirVector = playerTransform.position - transform.position;
            transform.rotation = Quaternion.Slerp
            (transform.rotation, Quaternion.LookRotation(dirVector), rotationSpeed * Time.deltaTime);
        }
        else
        {
            walking = false;
            attacking = true;
            Debug.Log("Im supposed to be attacking");
        }

        basicZombieAnimator.SetBool("walking", walking);
        basicZombieAnimator.SetBool("attacking", attacking);

    }

    public bool IsMoving()
    {
        return !(Vector3.Distance(transform.position, playerTransform.position) < agent.stoppingDistance);
    }
}
