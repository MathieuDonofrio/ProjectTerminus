using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class GunController : MonoBehaviour
{

    /* Configuration */

    [Header("Information")]
    [Tooltip("The name of the weapon")]
    public string weaponName = "Unknown";

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

    [Tooltip("Percentage amount of fire delay time used to shoot all rounds")]
    public float burstTime = 0.6f;

    [Tooltip("Fire delay is multiplied by this value if jitter clicking")]
    public float semiFireDelayModifier = 0.5f;

    [Header("Recoil Settings")]
    [Tooltip("Recoil system used to apply recoil")]
    public RecoilSystem recoilSystem;

    [Tooltip("Amount of spread applied when hipfiring")]
    public float spreadHip = 1;

    [Tooltip("Amount of spread applied when aiming down sight")]
    public float spreadAim = 0.1f;

    [Tooltip("Amount of recoil applied after each shot")]
    public float recoil = 1;

    [Tooltip("How fast the gun will apply its recoil in seconds")]
    public float kickSpeed = 0.05f;

    [Tooltip("How fast the gun will recenter its recoil in seconds")]
    public float centerSpeed = 0.2f;

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

    private PlayerInputHandler inputHandler;

    /* State */

    [Header("Debug")]

    public int Clip;

    public int reloadAmt;

    public bool IsReloading;

    public bool IsFiring;

    /* Timestamps */

    public float lastShot;

    public float lastRound;

    public float lastReload;

    /* Counters */

    public float individualReloadCounter;

    public float roundDelayCounter;

    public float roundCounter;


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
        if (CheckFiringConditions())
        {
            if (firingType == FiringType.BURST && roundsPerBurst > 1)
            {
                roundDelayCounter += Time.deltaTime;

                float delay = fireDelay * burstTime / roundsPerBurst;

                while (roundDelayCounter >= delay || !IsFiring)
                {
                    if (roundCounter++ < roundsPerBurst)
                    {
                        Fire();

                        roundDelayCounter -= delay;
                    }
                    else
                    {
                        FinishFiring();

                        break;
                    }
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
        if (IsReloading)
        {
            if (reloadType == ReloadType.INDIVIDUAL && reloadAmt > 0)
            {
                if (!inputHandler.GetFireInput())
                {
                    individualReloadCounter += Time.deltaTime;

                    float reloadFrac = (reloadTime - 0.01f) / maxClipSize;

                    while (individualReloadCounter >= reloadFrac)
                    {
                        individualReloadCounter -= reloadFrac;

                        Clip++;

                        reloadAmt--;
                    }
                }
                else
                {
                    StopReloading();
                }
            }

        }

        // Finish reloading
        if (IsReloading && (reloadAmt == 0 || Time.time - lastReload >= reloadTime))
        {
            FinishReloading();
        }

    }

    /* Services */

    public bool Shoot()
    {
        // TODO

        if(recoilSystem != null)
        {
            // TODO make better (Add spray patterns ?)

            // Calculate recoil kick
            Vector2 kick = Vector2.up * recoil;

            // Apply kick
            recoilSystem.Kick(kick);
        }

        return true;
    }

    public void Fire()
    {
        // Check clip amount
        if (IsClipEmpty())
            return;

        // Set firing to true
        IsFiring = true;

        // Call shoot
        if (Shoot()) Clip--;

        // Record last shot
        lastShot = Time.time;
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

        // Finish firing
        FinishFiring();

        // Set reload amount
        reloadAmt = Mathf.Min(amount, maxClipSize);

        // Record last reload
        lastReload = Time.time;
    }

    public void StopReloading()
    {
        // Set amount to 0
        reloadAmt = 0;

        // Reset individual reload counter
        individualReloadCounter = 0;

        // Set reloading to false
        IsReloading = false;
    }

    public void FinishReloading()
    {
        // Finalize reload
        Clip += reloadAmt;

        StopReloading();
    }

    public bool CheckFiringConditions()
    {
        // Check holding fire
        bool fire = inputHandler.GetFireInput();

        // Get firing delay
        float delay = firingType == FiringType.SEMI_AUTO
            ? fireDelay * semiFireDelayModifier : fireDelay;

        // Enable jitter clicking for semi auto and burst
        if (firingType == FiringType.SEMI_AUTO || firingType == FiringType.BURST)
        {
            fire |= inputHandler.GetFireDownInput(delay * 0.8f);
        }

        // Check delay
        fire &= Time.time - lastShot >= delay;

        // Check if already firing
        fire |= IsFiring;

        // Check if reloading
        fire &= !IsReloading;

        // Check clip
        fire &= Clip > 0;

        return fire;
    }

    public bool IsClipEmpty()
    {
        return Clip <= 0;
    }

}

public enum FiringType
{
    AUTO, SEMI_AUTO, BURST
}

public enum ReloadType
{
    MAGAZINE, INDIVIDUAL
}
