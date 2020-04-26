using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
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

    /* Required Components */

    private PlayerInputHandler inputHandler;

    /* State */

    [Header("Debug")]

    public bool holdingPrimaryGun;

    public int primaryAmmo;

    public int secondaryAmmo;

    private void Start()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
    }



}
