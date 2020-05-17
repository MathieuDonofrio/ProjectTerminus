using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(ZombieAudioManager))]
[RequireComponent(typeof(Collider))]
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

    [Tooltip("Amount of seconds the spawning lasts")]
    public float spawnDuration = 2.0f;

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

    private ZombieAudioManager audioManager;

    /* State */

    public Entity TargetEntity { get; private set; }

    public bool IsAttacking { get; private set; }

    public bool IsSpawning { get; set; }

    public bool Enabled { get; set; }

    /* Timestamps */

    private float lastAttackTime = Mathf.NegativeInfinity;

    private float lastSpawnTime = Mathf.NegativeInfinity;
    
    private float lastScreamTime = Mathf.NegativeInfinity;

    /* Other */

    private bool attackSuccessfull;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        entity = GetComponent<Entity>();
        audioManager = GetComponent<ZombieAudioManager>();

        entity.onDeath += OnDeath;

        StartSpawnZombie();
    }

    private void FixedUpdate()
    {
        if (!enabled)
            return;

        if (!entity.IsDead && TargetEntity != null)
        {
            UpdateSpeed();

            if(!IsAttacking && agent.destination != TargetEntity.transform.position && agent.enabled)
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

            HandleScreams();
        }

        if (IsSpawning && Time.time - lastSpawnTime >= spawnDuration)
        {
            FinishSpawnZombie();
        }

    }

    private void StartAttack()
    {
        // Stop agent
        if (agent.enabled)
        {
            agent.SetDestination(transform.position);
        }

        // Update state
        IsAttacking = true;

        // Toggle walking animation and handle sound
        animator.SetBool("attacking", true);
        animator.SetBool("walking", false);

        audioManager.StopWalking();
        audioManager.PlayAttack();

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

        audioManager.PlayWalking();
    }

    public void StartSpawnZombie()
    {
        animator.SetBool("rising", true);

        lastSpawnTime = Time.time;

        IsSpawning = true;
    }

    public void FinishSpawnZombie()
    {
        IsSpawning = false;

        SetTargetNearestPlayer();

        animator.SetBool("rising", false);
        animator.SetBool("walking", true);

        audioManager.PlayWalking();
    }

    private void UpdateSpeed()
    {
        float speed = movementSpeed * rage;

        if (agent.speed != speed) agent.speed = speed;
        if (animator.speed != rage) animator.speed = rage;
    }

    private void HandleScreams()
    {
        if (Time.time - lastScreamTime >= 4 && Random.value > 0.01)
        {
            audioManager.PlayWalking();

            lastScreamTime = Time.time;
        }
    }

    private void OnDeath()
    {
        agent.enabled = false;

        audioManager.StopWalking();
        audioManager.PlayDeath();
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

    public bool IsDead()
    {
        return entity == null ? false : entity.IsDead;
    }

    public void Revive()
    {
        entity.Revive();
    }

}
