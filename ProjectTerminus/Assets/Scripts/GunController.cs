using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class GunController : MonoBehaviour
{

    /* Configuration */

    [Header("Information")]
    [Tooltip("The name of the weapon")]
    public string weaponName;

    [Header("Projectile Settings")]
    [Tooltip("The projectile that will be shot from the gun")]
    public Projectile projectilePrefrab;

    [Header("Shoot Settings")]
    [Tooltip("The firing mechanism type of the gun")]
    public FiringType firingType = FiringType.AUTO;

    [Tooltip("Amount of projectiles per shot")]
    public int projectilesPerShot = 1;

    [Tooltip("Amount of rounds per shot")]
    public int roundsPerBurst = 1;

    [Tooltip("Amount of time between shots")]
    public float fireDelay = 0.5f;

    [Tooltip("Amount of time between rounds. Applies only if roundsPerBurst > 1")]
    public float roundDelay = 0.1f;

    [Tooltip("Amount of spread applied when hipfiring")]
    public float spreadHip = 1;

    [Tooltip("Amount of spread applied when aiming down sight")]
    public float spreadAim = 0.1f;

    [Tooltip("Amount of recoil applied after each shot")]
    public float recoil = 1;

    [Tooltip("How fast the gun will recenter its recoil")]
    public float centerSpeed = 1;

    [Header("Reload Settings")]
    [Tooltip("The type of reloading mechanisim")]
    public ReloadType reloadType = ReloadType.MAGAZINE;

    [Tooltip("Amount of time in seconds to reload gun compleately")]
    public float reloadTime = 3f;

    [Tooltip("Amount of bullets in clip")]
    public int maxClipSize = 30;

    [Header("Movement Settings")]
    [Tooltip("Movement speed is multiplied by this value")]
    public float movementMultiplier = 1.0f;

    [Tooltip("Movement speed is multiplied by this value when aiming down sight")]
    public float movementAimingModifier = 0.8f;

    [Header("Damage Settings")]
    [Tooltip("Base amount of damage to be applied when hit by projectile comming from this gun")]
    public float damage = 2;

    [Tooltip("Range of random amount of damage added to base damage")]
    public float randomDamage = 1;

    [Tooltip("Damage is multiplied by this value if the entity what shot in the head")]
    public float headshotModifier = 1.5f;

    /* Required Components */

    public PlayerInputHandler inputHandler;

    /* State */

    public int Clip { get; private set; }

    public bool IsReloading { get; private set; }

    public bool IsFiring { get; private set; }

    /* Timestamps */

    public float lastShot;

    public float lastRound;

    public float lastReload;

    /* Counters */

    public float individualReloadCounter;

    public float roundDelayCounter;

    public float roundCounter;

    /* Other */

    public int reloadAmt;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {

        HandleReloading();
        HandleFiring();

    }

    /* Handlers */

    private void HandleFiring()
    {
        if (HoldingFire() && Time.time - lastShot >= fireDelay || IsFiring && !IsReloading)
        {
            if(roundsPerBurst > 1)
            {
                roundDelayCounter += Time.deltaTime;

                while (roundDelayCounter >= roundDelay)
                {
                    roundCounter++;

                    if(roundCounter++ < roundsPerBurst)
                    {
                        Fire();
                    }
                    else 
                    {
                        FinishFiring();
                    }

                    roundDelayCounter -= roundDelay;

                    Clip++;
                }
            }
            else
            {
                Fire();
                FinishFiring();
            }
        }

        // Stop firing if reloading
        if (IsReloading) IsFiring = false;
    }

    private void HandleReloading()
    {
        if(reloadType == ReloadType.INDIVIDUAL)
        {
            individualReloadCounter += Time.deltaTime;
                 
            float reloadFrac = (reloadTime - 0.01f) / maxClipSize;

            while(individualReloadCounter >= reloadFrac)
            {
                individualReloadCounter -= reloadFrac;

                Clip++;
            }
        }

        // Finish reloading
        if (Time.time - lastReload >= reloadTime) FinishReloading();
    }

    /* Services */

    public void Shoot()
    {
        // TODO
    }

    public void Fire()
    {
        // Set firing to true
        IsFiring = true;

        // Call shoot
        Shoot();
    }

    public void FinishFiring()
    {
        // Set firing to false
        IsFiring = false;

        // Reset round delay counter
        roundDelayCounter = 0;

        // Reset round counter
        roundCounter = 0;
    }

    public void Reload(int amount)
    {
        // Check reload amount
        if (amount < 1)
            return;

        // Set reloading to true
        IsReloading = true;

        // Set reload amount
        reloadAmt = Mathf.Min(amount, maxClipSize);

        // Record last reload
        lastReload = Time.time;
    }

    public void FinishReloading()
    {
        // Set reloading to false
        IsReloading = false;

        // Set amount to 0
        reloadAmt = 0;

        // Set clip
        if(reloadType == ReloadType.MAGAZINE) Clip = reloadAmt;

        // Reset individual reload counter
        individualReloadCounter = 0;
    }
    
    public bool HoldingFire()
    {
        return false; // TODO link to input
    }

}

public enum FiringType
{
    AUTO, SEMI_AUTO
}

public enum ReloadType
{
    MAGAZINE, INDIVIDUAL
}
