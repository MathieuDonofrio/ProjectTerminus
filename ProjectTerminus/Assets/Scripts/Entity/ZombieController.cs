using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Entity))]
public class ZombieController : MonoBehaviour
{
    /* Configuration */

    [Header("Movement Settings")]
    [Tooltip("Base movement speed")]
    public float movementSpeed = 0.4f;

    [Tooltip("Speed multiplier for movement and animations")]
    public float rage = 1.0f;

    [Header("Target Settings")]
    [Tooltip("The maximum range the zombie can attack from")]
    public float attackRange = 2.0f;

    [Tooltip("The attack range the zombie will start attacking from")]
    public float startAttackRange = 1.0f;

    [Tooltip("Amount of seconds for head rotation to look at a position")]
    public float lookSpeed = 0.2f;

    [Header("Attack Settings")]
    [Tooltip("Minimum amount of seconds between each attack")]
    public float attackDelay= 3f;

    [Tooltip("Amount of seconds the attack lasts")]
    public float attackDuration = 2.0f;

    [Tooltip("When the attack hit will ")]
    public float attackHitDelay = 1.0f;

    [Tooltip("Amount of base damage per attack")]
    public float attackDamage = 2.0f;

    [Tooltip("Amount of random damage added to base damage per attack")]
    public float randomAttackDamage = 1.0f;

    /* Required Components */

    private NavMeshAgent agent;

    private Entity entity;

    private Animator animator;

    private RagDollController ragDollController;

    /* State */

    public Entity TargetEntity { get; private set; }

    public bool IsAttacking { get; private set; }

    public bool Enabled { get; set; }

    /* Timestamps */

    private float lastAttackTime = Mathf.NegativeInfinity;

    /* Other */

    private bool attackSuccessfull;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        entity = GetComponent<Entity>();
        ragDollController = GetComponent<RagDollController>();

        entity.onDeath += OnDeath;

        SetTargetNearestPlayer();

        animator.SetBool("walking", true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entity.Kill();
        }
    }

    private void FixedUpdate()
    {
        if (!enabled)
            return;

        if (!entity.IsDead && TargetEntity != null)
        {

            UpdateSpeed();

            if(!IsAttacking && agent.destination != TargetEntity.transform.position)
            {
                agent.SetDestination(TargetEntity.transform.position);
            }

            if (IsAttacking)
            {
                if(!IsWithinAttackRange(attackRange) || Time.time - lastAttackTime >= attackDuration / rage)
                {
                    FinishAttack();
                }
                else if (!attackSuccessfull && Time.time - lastAttackTime >= attackHitDelay / rage)
                {
                    Attack();
                }

            }
            else if(Time.time - lastAttackTime >= attackDelay / rage && IsWithinAttackRange(startAttackRange))
            {
                StartAttack();
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
        agent.enabled = false;

        // Update state
        IsAttacking = false;

        // Stop animations
        animator.SetBool("walking", false);
        animator.SetBool("attacking", false);
        animator.enabled = false;
        //ragDollController.ActivateRagdoll(true);

        // Start death animation
        //zombieAnimator.SetBool("dying", true);
    }

    private void StartAttack()
    {
        // Stop agent
        agent.SetDestination(transform.position);

        // Update state
        IsAttacking = true;

        // Toggle walking animation
        animator.SetBool("attacking", true);
        animator.SetBool("walking", false);

        // Reset attack success flag
        attackSuccessfull = false;

        // Record last attack
        lastAttackTime = Time.time;
    }

    private void Attack()
    {
        // Calculate damage
        float damage = attackDamage + Random.value * randomAttackDamage;

        // Damage entity
        TargetEntity.Damage(damage, gameObject, DamageType.PHYSICAL);

        // Flag attack success
        attackSuccessfull = true;
    }

    private void FinishAttack()
    {
        IsAttacking = false;

        animator.SetBool("attacking", false);
        animator.SetBool("walking", true);
    }

    private void UpdateSpeed()
    {
        float speed = movementSpeed * rage;

        if (agent.speed != speed) agent.speed = speed;
        if (animator.speed != rage) animator.speed = rage;
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

    private bool IsWithinAttackRange(float attackRange)
    {
        if (TargetEntity == null)
            return false;

        // Calculate square distance
        float sqrDistance = (TargetEntity.transform.position - transform.position).sqrMagnitude;

        return sqrDistance < attackRange * attackRange;
    }

}
