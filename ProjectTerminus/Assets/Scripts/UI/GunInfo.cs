using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInfo : MonoBehaviour
{
    /* Configuration */

    [Tooltip("The gun holder")]
    public GunHolder gunHolder;

    [Tooltip("Text UI component for displaying the gun name")]
    public Text gunNameText;

    [Tooltip("Text UI component for displaying the gun ammo")]
    public Text gunAmmoText;

    public Color reloadingColor = Color.yellow;

    private void LateUpdate()
    {
        GunController gun = gunHolder.CurrentHeldGun();

        if(gun != null && gun.IsReloading)
        {
            float delta = Time.time - gun.lastReload;

            float t = Mathf.Sin(delta * gun.reloadTime * 1.5f);

            gunAmmoText.color = Color.Lerp(Color.white, reloadingColor, t);
        }
        else if(gunAmmoText.color != Color.white)
        {
            gunAmmoText.color = Color.white;
        }
    }

    /* Services */

    public void UpdateGunInfo(string gunName, int clip, int ammo)
    {
        if(gunNameText.text != gunName) gunNameText.text = gunName;

        string ammoInfo = clip + " | " + ammo;

        if (gunAmmoText.text != ammoInfo) gunAmmoText.text = ammoInfo;
    }
}
