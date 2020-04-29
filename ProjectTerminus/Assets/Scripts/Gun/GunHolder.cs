using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerController))]
public class GunHolder : MonoBehaviour
{
    /* Configuration */

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

    [Tooltip("The HUD controller")]
    public HUDController hudController;

    [Tooltip("The gun container")]
    public Transform gunContainer;

    /* Required Components */

    private PlayerInputHandler inputHandler;

    private PlayerController playerController;

    /* State */

    [Header("Debug")]

    public bool holdingPrimaryGun;

    public int primaryAmmo;

    public int secondaryAmmo;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        playerController = GetComponent<PlayerController>();

        SetGun(false);
    }

    private void OnDestroy()
    {
        SetGun(false);
    }

    private void Update()
    {
        GunController held = CurrentHeldGun();

        if (held != null)
        {
            if (!held.IsReloading && inputHandler.GetReloadInput() || (held.IsClipEmpty() && autoReload))
            {
                ReloadHeldGun();
            }

            if (hudController != null)
            {
                hudController.UpdateGunInfo(held.name, held.Clip, holdingPrimaryGun
                    ? primaryAmmo : secondaryAmmo);

                hudController.UpdateCrosshair(
                    held.spreadHip + held.recoil, 
                    playerController.velocity.magnitude,
                    CrosshairType.DEFAULT);
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
    /// Returns the normalized direction the gun holder is looking in.
    /// </summary>
    /// <returns>normalized look direction</returns>
    public Vector3 Direction()
    {
        return playerController.playerHead.transform.eulerAngles.normalized;
    }
}
