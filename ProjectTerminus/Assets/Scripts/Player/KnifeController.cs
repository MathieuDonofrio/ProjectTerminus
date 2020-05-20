using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class KnifeController : MonoBehaviour
{
    /* Configuration */

    [Tooltip("HUD controller")]
    public HUDController hudController;

    [Header("Attack settings")]
    [Tooltip("Range of attack")]
    public float attackRange = 2f;

    [Tooltip("Minimum delay between attacks")]
    public float attackDelay = 2f;

    [Tooltip("Duration of attack")]
    public float attackDuration = 0.25f;

    [Tooltip("Damage of attack")]
    public float damage = 5f;

    [Header("Audio Clips")]
    [Tooltip("Sound played when knife is slashing")]
    public AudioClip slashSFX;

    /* Required Components */

    private PlayerInputHandler inputHandler;

    private AudioSource audioSource;

    private Animator knifeAnimator;

    /* State */

    private float lastAttackTime = Mathf.NegativeInfinity;

    private bool attacking;

    private bool attackSuccessful;

    private int layerMask;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        audioSource = GetComponent<AudioSource>();
        knifeAnimator = GetComponent<Animator>();

        layerMask = LayerMask.GetMask("Entity");
    }

    private void Update()
    {
        if (attacking && !attackSuccessful)
        {
            CheckSlashCollision();
        }
    }

    private void FixedUpdate()
    {
        if (!attacking && inputHandler.GetKnifeInput() && Time.time - lastAttackTime >= attackDelay)
        {
            StartAttack();
        }

        if (attacking && Time.time - lastAttackTime >= attackDuration)
        {
            FinishAttack();
        }
    }

    private void CheckSlashCollision()
    {
        Vector3 position = transform.position - transform.forward;

        if(Physics.SphereCast(
            position, 0.2f, transform.forward, 
            out RaycastHit hit, 
            attackRange, layerMask, 
            QueryTriggerInteraction.Ignore))
        {
            SlashAttack(hit.collider.GetComponent<Entity>());
        }
    }

    private void SlashAttack(Entity entity)
    {
        // Do damage and get result
        bool killed = entity.Damage(damage, gameObject, DamageType.PHYSICAL);

        // Flag hit
        hudController.Hitmarker(killed);

        // Set attack successful
        attackSuccessful = true;
    }

    private void StartAttack()
    {
        // Trigger animation
        knifeAnimator.SetTrigger("attacking");

        // Play slash sound
        audioSource.PlayOneShot(slashSFX);
        
        // Set attacking
        attacking = true;

        // Record last attack
        lastAttackTime = Time.time;
    }

    private void FinishAttack()
    {
        // Reset success
        attackSuccessful = false;

        // Set not attacking
        attacking = false;
    }

}
