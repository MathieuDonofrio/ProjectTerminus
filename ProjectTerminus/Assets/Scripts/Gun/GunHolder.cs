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

    [Tooltip("The first person shooter camera")]
    public Camera fpsCamera;

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

    [Header("Guns")]
    [Tooltip("Glock 19")]
    public GunController glock19;

    [Tooltip("FAMAS")]
    public GunController famas;

    [Tooltip("P90")]
    public GunController p90;

    [Tooltip("SCAR-17")]
    public GunController scar17;

    [Tooltip("SPAS-12")]
    public GunController spas12;

    [Header("Audio Clips")]
    [Tooltip("Sound played when landed a hit")]
    public AudioClip hitmarker;

    /* Required Components */

    private PlayerInputHandler inputHandler;

    private PlayerController playerController;

    private AudioSource audioSource;

    private Economy economy;

    /* State */

    public bool holdingPrimaryGun;

    public int primaryAmmo;

    public int secondaryAmmo;

    public float lastAim;

    public bool aiming;

    public float baseFOV;

    public float gunFOV;

    public bool startingReload;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        economy = GetComponent<Economy>();

        baseFOV = fpsCamera.fieldOfView;

        SetGun(false);

        startingReload = true;
    }

    private void OnDestroy()
    {
        SetGun(false);
    }

    private void Update()
    {
        HandleSlotChange();
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

        GunController currentHeldGun = CurrentHeldGun();

        if (currentHeldGun == null)
            return;

        float aimSpeed = 1 / currentHeldGun.aimSpeed;

        if (aiming != aimInput)
        {
            aiming = aimInput;

            animator.speed = aimInput ? aimSpeed : 1;

            animator.SetBool("Scoped", aimInput);

            if (aimInput)
            {
                hudController.CrosshairTransition(CrosshairType.IRON_SIGHT, aimSpeed);
            }

            lastAim = Time.time;
        }

        {
            float delta = Time.time - lastAim;

            float FOVEffectSpeed = currentHeldGun.aimSpeed * 1.5f;

            if (aiming && delta <= FOVEffectSpeed + 0.05f)
            {
                gunFOV = Mathf.SmoothStep(gunFOV, currentHeldGun.fovDecrease, delta / FOVEffectSpeed);
            }
            
            if (!aiming && delta <= 0.2f)
            {
                gunFOV = Mathf.SmoothStep(gunFOV, 0, delta * 5);
            }
        }

        {
            float FOV = baseFOV - gunFOV;

            if (fpsCamera != null && fpsCamera.fieldOfView != FOV)
            {
                fpsCamera.fieldOfView = FOV;
            }
        }

    }

    private void UpdateReloading()
    {
        GunController held = CurrentHeldGun();

        if(held != null)
        {
            if (!held.IsReloading && (inputHandler.GetReloadInput() || (held.IsClipEmpty() && autoReload)))
            {
                ReloadHeldGun();
            }
        }

        if (startingReload)
        {
            secondaryGun.Reload(secondaryGun.maxClipSize, true);

            AmmoRefill();

            startingReload = false;
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
                        held.spreadHip + Mathf.Min(held.recoil, 2), MovementAccuracy(), CrosshairType.DEFAULT);
                }
            }

        }
        else
        {
            hudController.UpdateGunInfo("Empty Slot", 0, 0);
        }
    }

    private void HandleSlotChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetGun(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetGun(false);
        }

        if (inputHandler.GetMouseScrollWheel() != 0.0f)
        {
            SwitchGun();
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

                        if (gun.reloadType != ReloadType.INDIVIDUAL) primaryAmmo -= amount;

                        gun.Reload(amount);
                    }
                }
                else if (secondaryAmmo > 0)
                {
                    int amount = Mathf.Min(secondaryAmmo, needed);

                    if (gun.reloadType != ReloadType.INDIVIDUAL) secondaryAmmo -= amount;

                    gun.Reload(amount);
                }
            }
            else
            {
                gun.Reload(needed);
            }
        }
    }

    /// <summary>
    /// Adds a gun with the specified string
    /// </summary>
    /// <param name="gun"></param>
    public void AddGun(string gun)
    {
        if (gun == "Glock 19") AddGun(glock19);
        else if (gun == "FAMAS") AddGun(famas);
        else if (gun == "P90") AddGun(p90);
        else if (gun == "SCAR-17") AddGun(scar17);
        else if (gun == "SPAS-12") AddGun(spas12);
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
        else if (holdingPrimaryGun)
        {
            primaryGun.gameObject.SetActive(false);

            AddGun(gun, true);
        }
        else
        {
            secondaryGun.gameObject.SetActive(false);

            AddGun(gun, false);
        }

        gun.gameObject.SetActive(true);

        gun.gunHolder = this;
        gun.transform.parent = gunContainer;
    }

    /// <summary>
    /// Adds a gun and specifies what slot
    /// </summary>
    /// <param name="gun">the gun to set</param>
    /// <param name="primary">the slot to set gun to</param>
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

        // Reload gun
        gun.Reload(gun.maxClipSize, true);

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
    /// Reloads and refills all guns.
    /// </summary>
    public void AmmoRefill()
    {
        if(primaryGun != null)
        {
            primaryAmmo = (int)(primaryGun.maxClipSize * amountOfClipsPerRefill);
        }
        
        if(secondaryGun != null)
        {
            secondaryAmmo = (int)(secondaryGun.maxClipSize * amountOfClipsPerRefill);
        }
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

    /// <summary>
    /// Takes an individual bullet for currently held gun
    /// </summary>
    public void TakeIndividual()
    {
        if (holdingPrimaryGun)
        {
            primaryAmmo = Mathf.Max(primaryAmmo - 1, 0);
        }
        else
        {
            secondaryAmmo = Mathf.Max(secondaryAmmo - 1, 0);
        }
    }

}
