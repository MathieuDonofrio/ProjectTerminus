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
        if(gunAmmoText.text != gunName) gunNameText.text = gunName;

        string ammoInfo = clip + " | " + ammo;

        if (gunAmmoText.text != ammoInfo) gunAmmoText.text = ammoInfo;
    }
}
