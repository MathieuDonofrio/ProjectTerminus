using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(SprayPattern))]
[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviour
{

    /* Configuration */

    [Header("Information")]
    [Tooltip("The name of the weapon")]
    public string weaponName = "Unknown";

    [Tooltip("The gun holder")]
    public GunHolder gunHolder;

    [Header("Projectile Settings")]
    [Tooltip("The projectile pool for the gun")]
    public ProjectileHandler projectilePool;

    [Tooltip("The projectile exit point of the gun")]
    public Transform exitPoint;

    [Tooltip("The projectile maximum travel range")]
    public float range = 100;

    [Tooltip("The speed of the projectile comming out of the gun")]
    public float projectileSpeed = 100;

    [Tooltip("The amount of knockback the projectile will do when it hits")]
    public float impactStrength = 0.5f;

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

    [Header("Recoil Settings")]
    [Tooltip("Recoil system used to apply recoil")]
    public RecoilSystem recoilSystem;

    [Tooltip("Gun kick system used to apply gun kick")]
    public GunKickSystem gunKickSystem;

    [Tooltip("Amount of angle of spread applied when hipfiring")]
    public float spreadHip = 20;

    [Tooltip("Amount of angle of spread applied when aiming down sight")]
    public float spreadAim = 0.1f;

    [Tooltip("Amount of recoil applied after each shot")]
    public float recoil = 1;

    [Tooltip("Amount of gun kick applied after each shot")]
    public float gunKick = 0.01f;

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

    [Header("Aiming Settings")]
    [Tooltip("How many seconds it takes for this gun to scope")]
    public float aimSpeed = 0.1f;

    [Tooltip("How much the field of view is decrease when scoped")]
    public float fovDecrease = 10;

    [Header("Damage Settings")]
    [Tooltip("Base amount of damage to be applied when hit by projectile comming from this gun")]
    public float damage = 2;

    [Tooltip("Range of random amount of damage added to base damage")]
    public float randomDamage = 1;

    [Tooltip("Damage is multiplied by this value if the entity what shot in the head")]
    public float headshotModifier = 1.5f;

    [Header("Audio Clips")]
    [Tooltip("Sound played when gun shoots")]
    public AudioClip shot;

    [Tooltip("Sound played when magazine is taken out")]
    public AudioClip magOut;

    [Tooltip("Sound played when magazine in put in")]
    public AudioClip magIn;

    [Tooltip("Sound played a individual round is loaded")]
    public AudioClip individualIn;

    [Tooltip("Sound player is trying to shoot with empty clip")]
    public AudioClip outOfAmmo;

    /* Required Components */

    private PlayerInputHandler inputHandler;

    private SprayPattern sprayPattern;

    private AudioSource audioSouce;

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

    public int consecutiveShotCounter;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        sprayPattern = GetComponent<SprayPattern>();
        audioSouce = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        Profiler.BeginSample("GunControllerLateUpdate");
#endif

        HandleReloading();
        HandleFiring();

#if UNITY_EDITOR
        Profiler.EndSample();
#endif
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

        if(consecutiveShotCounter > 0 && Time.time - lastShot >= 2 * fireDelay)
        {
            consecutiveShotCounter = 0;
        }

        // Stop firing if reloading
        if (IsReloading)
        {
            IsFiring = false;
        }
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

                        gunHolder.TakeIndividual();

                        audioSouce.PlayOneShot(individualIn);
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

    public void Shoot()
    {

        if (recoilSystem != null)
        {
            // Calculate recoil kick
            Vector2 kick = sprayPattern.getRecoil(consecutiveShotCounter) * recoil;

            // Apply recoil kick
            recoilSystem.Kick(kick);

            // Apply gun kick
            gunKickSystem.Kick(gunKick, Mathf.Min(fireDelay, 0.2f));
        }

        // Calculate ray start
        Vector3 rayStart = exitPoint.position - exitPoint.transform.forward;

        // Calculate position
        Vector3 position = exitPoint.position + gunHolder.Movement() * Time.deltaTime;

        // Calculate ranged direction
        Vector3 rangedDirection = transform.forward * range;

        // Calculate deviation amount
        float deviationAmount = gunHolder.aiming ? spreadAim : spreadHip + gunHolder.MovementAccuracy();

        for (int i = 0; i < projectilesPerShot; i++)
        {
            // Calculate random deviation
            Vector3 deviation = Random.onUnitSphere * deviationAmount;

            // Calculate rotation
            Quaternion rotation = Quaternion.LookRotation(rangedDirection + deviation);

            // Launch projectile
            projectilePool.LaunchProjectile(this, rayStart, position, rotation, range, projectileSpeed);
        }

        // Spawn muzzle flash
        projectilePool.SpawnMuzzleFlash(position, transform.eulerAngles, gunHolder.Movement());

        // Play shot sound
        audioSouce.PlayOneShot(shot);

        // Increment consecutive shots
        consecutiveShotCounter++;

        // Record last shot
        lastShot = Time.time;
    }

    public bool CheckFiringConditions()
    {
        // Check holding fire
        bool fire;

        if (firingType == FiringType.SEMI_AUTO)
        {
            fire = inputHandler.GetFireDownInput(fireDelay * 0.8f);
        }
        else if(firingType == FiringType.BURST)
        {
            fire = inputHandler.GetFireInput() || inputHandler.GetFireDownInput(fireDelay * 0.5f);
        }
        else
        {
            fire = inputHandler.GetFireInput();
        }

        // Check delay
        fire &= Time.time - lastShot >= fireDelay;

        // Check if already firing
        fire |= IsFiring;

        // Check if reloading
        fire &= !IsReloading;

        // Check clip
        fire &= Clip > 0;

        return fire;
    }

    public void Fire()
    {
        if (!IsClipEmpty())
        {
            // Set firing to true
            IsFiring = true;

            // Call shoot
            Shoot();

            // Decrement clip
            Clip--;
        }
        else
        {
            // Play out of ammo
            audioSouce.PlayOneShot(outOfAmmo);
        }
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

    public void Reload(int amount, bool instant = false)
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

        // Reset consecutive shot counter
        consecutiveShotCounter = 0;

        // Play mag out
        if (reloadType == ReloadType.MAGAZINE) audioSouce.PlayOneShot(magOut);

        // Record last reload
        lastReload = Time.time;

        if (instant) FinishReloading();
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

        // Play mag in
        if (reloadType == ReloadType.MAGAZINE) audioSouce.PlayOneShot(magIn);

        StopReloading();
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
