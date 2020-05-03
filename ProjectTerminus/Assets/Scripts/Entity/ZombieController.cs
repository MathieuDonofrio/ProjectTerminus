using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Entity))]
public class ZombieController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Entity entity;
    private int rotationSpeed = 10;
    private Animator zombieAnimator;
    private bool walking = false;
    private bool attacking = false;
    private float initialSpeed;
    private bool canWalk = false;
    public readonly float minTimeRange = 1;
    public readonly float maxTimeRange = 6;
    public Entity playerEntity;
    private float attackSpeed = 1f;
    private float lastAttack;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        entity = GetComponent<Entity>();
        initialSpeed = agent.speed;
        entity.onDeath += OnDeath;
        Invoke("ToggleCanWalk", Random.Range(minTimeRange, maxTimeRange));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.Kill();
        }

        if (!entity.IsDead && canWalk)
        {

            // MOVE OUR AGENT
            agent.SetDestination(playerEntity.transform.position);


            //executing an action depending on if our player is moving
            if (IsWithinAttackRange() && Time.time - lastAttack >= attackSpeed)
            {
                //Set the animations
                walking = false;
                attacking = true;
                agent.speed = 0;
                AttackPlayer();
            }
            else
            {
                //Set the animations
                walking = true;
                attacking = false;
                agent.speed = initialSpeed;
            }

            //Make the character rotate
            var dirVector = (playerEntity.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVector), rotationSpeed * Time.deltaTime);
            zombieAnimator.SetBool("walking", walking);
            zombieAnimator.SetBool("attacking", attacking);
        }

    }

    public void ToggleCanWalk()
    {
        canWalk = !canWalk;
    }

    public bool IsWithinAttackRange()
    {
        return Vector3.Distance(transform.position, playerEntity.transform.position) < agent.stoppingDistance;
    }

    public void OnDeath()
    {
       //stop moving the character this is for test purposes
       agent.isStopped = true;
       walking = false;
       attacking = false;
       zombieAnimator.SetBool("walking", walking);
       zombieAnimator.SetBool("attacking", attacking);
       //zombieAnimator.SetBool("dying", true);
        Debug.Log("Im dead");
    }

    public void AttackPlayer()
    {
        playerEntity.Damage(3f,gameObject,DamageType.PHYSICAL);
        lastAttack = Time.time;
    }
}
