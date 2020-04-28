using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInfo : MonoBehaviour
{

    [Tooltip("Text UI component for displaying the gun name")]
    public Text gunNameText;

    [Tooltip("Text UI component for displaying the gun ammo")]
    public Text gunAmmoText;

    public void UpdateGunInfo(string gunName, int clip, int ammo)
    {
        gunNameText.text = gunName;

        gunAmmoText.text = clip + " | " + ammo;
    }
}
