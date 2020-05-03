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
    public float attackSpeed = 5f;
    public float stopTimeAfterAttack = 2f;
    private bool hasAttackForTheFirtsTime = false;
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
          //  entity.Kill();
        }

        if (!entity.IsDead && canWalk)
        {

            // MOVE OUR AGENT
            agent.SetDestination(playerEntity.transform.position);

            //make the player wait before applying damage for the first time


            //executing an action depending on if our player is moving
            if (IsWithinAttackRange() && Time.time - lastAttack >= attackSpeed)
            {
                //Set the animations
                walking = false;
                attacking = true;
                agent.speed = 0;
                AttackPlayer();
                //make the entity stop moving for a while after an attack
                Invoke("ToggleSpeed", stopTimeAfterAttack);
            }
            else
            {
                //Set the animations
                walking = true;
                attacking = false;
            }

            //Make the character rotate
            var dirVector = (playerEntity.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirVector), rotationSpeed * Time.deltaTime);
            zombieAnimator.SetBool("walking", walking);
            zombieAnimator.SetBool("attacking", attacking);
        }

    }

    private void ToggleCanWalk()
    {
        canWalk = !canWalk;
    }

    private bool IsWithinAttackRange()
    {
        return Vector3.Distance(transform.position, playerEntity.transform.position) < agent.stoppingDistance;
    }

    private void OnDeath()
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

    private void AttackPlayer()
    {
        playerEntity.Damage(entity.damageMultiplier,gameObject,DamageType.PHYSICAL);
        lastAttack = Time.time;
    }

    private void ToggleSpeed()
    {
        agent.speed = initialSpeed;
        hasAttackForTheFirtsTime = false;
    }
}
