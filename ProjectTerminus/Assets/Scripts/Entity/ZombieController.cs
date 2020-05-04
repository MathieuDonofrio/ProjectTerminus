using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(AudioSource))]
public class ZombieController : MonoBehaviour
{
    /* Configuration */

    [Header("Movement Settings")]
    [Tooltip("Base movement speed")]
    public float movementSpeed = 0.4f;

    [Tooltip("Speed multiplier for movement and animations")]
    public float rage = 1.0f;

    [Tooltip("The amount of time the zombie will be stunned after being shot")]
    public float shotStunDuration = 0.2f;

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

    [Header("Audio Clips")]
    [Tooltip("Sound played when zombie gets hurt")]
    public AudioClip hurt;

    /* Required Components */

    private NavMeshAgent agent;

    private Entity entity;

    private Animator animator;

    private AudioSource audioSouce;

    /* State */

    public Entity TargetEntity { get; private set; }

    public bool IsAttacking { get; private set; }

    public bool Enabled { get; set; }

    /* Timestamps */

    private float lastAttackTime = Mathf.NegativeInfinity;

    private float lastShotTime = Mathf.NegativeInfinity;

    /* Other */

    private bool attackSuccessfull;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        entity = GetComponent<Entity>();
        audioSouce = GetComponent<AudioSource>();

        entity.onDeath += OnDeath;
        entity.onDamaged += OnDamage;

        SetTargetNearestPlayer();

        animator.SetBool("walking", true);
    }

    private void FixedUpdate()
    {
        if (!enabled)
            return;

        if (!entity.IsDead && TargetEntity != null)
        {

            UpdateSpeed();

            if(!IsAttacking && !agent.isStopped && agent.destination != TargetEntity.transform.position)
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

            if(agent.isStopped && Time.time - lastShotTime >= shotStunDuration)
            {
                agent.isStopped = false;
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
        animator.SetBool("walking", false);
        animator.SetBool("attacking", false);

        // Start death animation
        //animator.SetBool("dying", true);

        Destroy(gameObject); // TODO temporary
    }

    private void OnDamage(float damage, GameObject shooter, DamageType type)
    {
        if(type == DamageType.PROJECTILE)
        {
            agent.isStopped = true;

            audioSouce.PlayOneShot(hurt);

            lastAttackTime = Time.time;
        }
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
