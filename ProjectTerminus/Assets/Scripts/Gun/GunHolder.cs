using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Economy))]
public class GunHolder : MonoBehaviour
{
    /* Configuration */

    [Tooltip("The gun container")]
    public Transform gunContainer;

    [Tooltip("The gun animator")]
    public Animator animator;

    [Header("Accuracy")]
    [Tooltip("How much the movement affects the accuracy")]
    public float movementAccuracy = 2;

    [Header("Loadout")]
    [Tooltip("Main gun for gunholder")]
    public GunController primaryGun;

    [Tooltip("Secondary gun for gunholder")]
    public GunController secondaryGun;

    [Header("Reload Settings")]
    [Tooltip("If true the gun holder will automatically reload the gun if empty and has available ammo")]
    public bool autoReload = true;

    [Tooltip("If true the gun holder will have unlimited ammo")]
    public bool unlimitedAmmo = true;

    [Tooltip("This number will multiply the gun clip size for")]
    public float amountOfClipsPerRefill = 6;

    [Header("HUD")]
    [Tooltip("The HUD controller")]
    public HUDController hudController;

    [Header("Audio Clips")]
    [Tooltip("Sound played when landed a hit")]
    public AudioClip hitmarker;

    /* Required Components */

    private PlayerInputHandler inputHandler;

    private PlayerController playerController;

    private AudioSource audioSource;

    private Economy economy;

    /* State */

    [Header("Debug")]

    public bool holdingPrimaryGun;

    public int primaryAmmo;

    public int secondaryAmmo;

    public bool aiming;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        economy = GetComponent<Economy>();

        SetGun(false);
    }

    private void OnDestroy()
    {
        SetGun(false);
    }

    private void Update()
    {
        UpdateReloading();
    }

    private void LateUpdate()
    {
        UpdateHud();
    }

    private void FixedUpdate()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        bool aimInput = inputHandler.GetAimInput();

        if(aiming != aimInput)
        {
            GunController currentHeldGun = CurrentHeldGun();

            if(currentHeldGun != null)
            {
                aiming = aimInput;

                float aimSpeed = 1 / currentHeldGun.aimSpeed;

                animator.speed = aimInput ? aimSpeed : 1;

                animator.SetBool("Scoped", aimInput);

                if (aimInput)
                {
                    hudController.CrosshairTransition(CrosshairType.IRON_SIGHT, aimSpeed);
                }

            }
        }

    }

    private void UpdateReloading()
    {
        GunController held = CurrentHeldGun();

        if(held != null)
        {
            if (!held.IsReloading && inputHandler.GetReloadInput() || (held.IsClipEmpty() && autoReload))
            {
                ReloadHeldGun();
            }
        }
    }

    private void UpdateHud()
    {
        GunController held = CurrentHeldGun();

        if (held != null)
        {

            if (hudController != null)
            {
                hudController.UpdateGunInfo(held.name, held.Clip, holdingPrimaryGun
                    ? primaryAmmo : secondaryAmmo);

                if (!aiming)
                {
                    hudController.UpdateCrosshair(
                        held.spreadHip + held.recoil, MovementAccuracy(), CrosshairType.DEFAULT);
                }
            }

        }
        else
        {
            hudController.UpdateGunInfo("Empty Slot", 0, 0);
        }
    }

    /* Services */

    /// <summary>
    /// Reloads the currently held gun
    /// </summary>
    public void ReloadHeldGun()
    {
        GunController gun = CurrentHeldGun();

        if(gun != null && !gun.IsReloading)
        {
            int needed = gun.maxClipSize - gun.Clip;

            if (!unlimitedAmmo)
            {
                if (holdingPrimaryGun)
                {
                    if (primaryAmmo > 0)
                    {
                        int amount = Mathf.Min(primaryAmmo, needed);

                        primaryAmmo -= amount;

                        gun.Reload(amount);
                    }
                }
                else
                {
                    if (secondaryAmmo > 0)
                    {
                        int amount = Mathf.Min(secondaryAmmo, needed);

                        secondaryAmmo -= amount;

                        gun.Reload(amount);
                    }
                }
            }
            else
            {
                gun.Reload(needed);
            }
        }
    }

    /// <summary>
    /// Adds the gun into the held gun slot. 
    /// If a slot of empty the gun will be added to the empty slot
    /// and the current held slot will to switched.
    /// </summary>
    /// <param name="gun">the gun to add</param>
    public void AddGun(GunController gun)
    {
        if (primaryGun == null) AddGun(gun, true);
        else if (secondaryGun == null) AddGun(gun, false);
        else if (holdingPrimaryGun) AddGun(gun, true);
        else AddGun(gun, false);

        gun.gunHolder = this;
        gun.transform.parent = gunContainer;
    }

    public void AddGun(GunController gun, bool primary)
    {
        // Calculate full refill ammo
        int ammo = (int)(gun.maxClipSize * amountOfClipsPerRefill);

        // Assign the new gun
        if (primary)
        {
            primaryGun = gun;
            primaryAmmo = ammo;
        }
        else
        {
            secondaryGun = gun;
            secondaryAmmo = ammo;
        }

        // Set the current gun to primary slot
        SetGun(primary);
    }

    /// <summary>
    /// Sets the current gun
    /// </summary>
    /// <param name="primary">true for primary, false for secondary</param>
    public void SetGun(bool primary)
    {
        if (primaryGun != null) primaryGun.gameObject.SetActive(primary);
        if (secondaryGun != null) secondaryGun.gameObject.SetActive(!primary);

        holdingPrimaryGun = primary;
    }

    /// <summary>
    /// Changed the currently held gun
    /// </summary>
    public void SwitchGun()
    {
        SetGun(!holdingPrimaryGun);
    }

    /// <summary>
    /// Return the currently held gun controller
    /// </summary>
    /// <returns>currently held gun controller</returns>
    public GunController CurrentHeldGun()
    {
        return holdingPrimaryGun ? primaryGun : secondaryGun;
    }

    /// <summary>
    /// Returns the movement accuracy of the gun holder
    /// </summary>
    /// <returns>normalized look direction</returns>
    public float MovementAccuracy()
    {
        return playerController.velocity.magnitude / playerController.movementSpeed * movementAccuracy;
    }

    /// <summary>
    /// Returns the movement of the gun holder
    /// </summary>
    /// <returns></returns>
    public Vector3 Movement()
    {
        Vector3 velocity = playerController.velocity;

        if (playerController.IsGrounded) velocity.y = 0;

        return velocity;
    }

    /// <summary>
    /// Flags a hit and updates the hud controller accordingly
    /// </summary>
    /// <param name="kill">if the kill resulted in a kill</param>
    public void LandedHit(bool kill, bool head)
    {
        hudController.Hitmarker(kill);

        audioSource.PlayOneShot(hitmarker);

        int points = kill ? head ? 70 : 40 : 10;

        economy.Transaction(points);
        hudController.UpdateMoney(economy.balance, points);
    }

}
