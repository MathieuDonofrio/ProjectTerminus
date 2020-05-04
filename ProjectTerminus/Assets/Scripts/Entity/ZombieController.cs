using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Entity))]
public class ZombieController : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Amount of seconds for head rotation to look at a position")]
    public float lookSpeed = 0.2f;

    [Header("Attack Settings")]
    [Tooltip("Minimum amount of seconds between each attack")]
    public float attackDelay= 3f;

    [Tooltip("Amount of seconds the attack lasts")]
    public float attackDuration = 2.0f;

    [Tooltip("Amount of base damage per attacl")]
    public float attackDamage = 2.0f;

    [Tooltip("Amount of random damage added to base damage per attack")]
    public float randomAttackDamage = 1.0f;

    /* Required Components */

    private NavMeshAgent agent;

    private Entity entity;

    private Animator zombieAnimator;

    /* State */

    public Entity TargetEntity { get; private set; }

    public bool IsAttacking { get; private set; }

    public bool Enabled { get; set; }

    /* Timestamps */

    private float lastAttackTime = Mathf.NegativeInfinity;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        entity = GetComponent<Entity>();

        entity.onDeath += OnDeath;

        SetTargetNearestPlayer();

        zombieAnimator.SetBool("walking", true);
    }

    private void FixedUpdate()
    {
        if (!enabled)
            return;

        if (!entity.IsDead && TargetEntity != null)
        {

            if(!IsAttacking && agent.destination != TargetEntity.transform.position)
            {
                agent.SetDestination(TargetEntity.transform.position);
            }

            if (IsWithinAttackRange() && Time.time - lastAttackTime >= attackDelay)
            {
                Attack(TargetEntity);
            }

            if(IsAttacking && Time.time - lastAttackTime >= attackDuration)
            {
                FinishAttack();
            }

            Vector3 targetDirection = TargetEntity.transform.position - transform.position;

            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(targetDirection), Time.fixedDeltaTime / lookSpeed);
        }

    }

    private void OnDeath()
    {
        // Stop agent
        agent.isStopped = true;

        // Update state
        IsAttacking = false;

        // Stop animations
        zombieAnimator.SetBool("walking", false);
        zombieAnimator.SetBool("attacking", false);

        // Start death animation
        //zombieAnimator.SetBool("dying", true);
    }

    private void FinishAttack()
    {
        IsAttacking = false;

        zombieAnimator.SetBool("attacking", false);
        zombieAnimator.SetBool("walking", true);
    }

    /* Services */

    public void SetTargetNearestPlayer()
    {
        // Find player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject closest = SearchUtil.FindClosest(players, transform.position);

        if (closest != null)
        {
            Entity entityPlayer = closest.GetComponent<Entity>();

            if (entityPlayer != null)
            {
                SetTarget(entityPlayer);
            }
        }

    }

    public void SetTarget(Entity entity)
    {
        // Set target entity
        TargetEntity = entity;
    }

    private bool IsWithinAttackRange()
    {
        if (TargetEntity == null)
            return false;

        // Calculate square distance
        float sqrDistance = (TargetEntity.transform.position - transform.position).sqrMagnitude;

        return sqrDistance < agent.stoppingDistance * agent.stoppingDistance;
    }

    public void Attack(Entity entity)
    {
        // Calculate damage
        float damage = attackDamage + Random.value * randomAttackDamage;

        // Damage entity
        entity.Damage(damage, gameObject, DamageType.PHYSICAL);

        // Stop agent
        agent.SetDestination(transform.position);

        // Update state
        IsAttacking = true;

        // Toggle walking animation
        zombieAnimator.SetBool("attacking", true);
        zombieAnimator.SetBool("walking", false);

        // Record last attack
        lastAttackTime = Time.time;
    }

}
