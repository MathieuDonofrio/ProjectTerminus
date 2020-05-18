using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PurchaseMenu : MonoBehaviour
{
    /* Configuration */

    public Canvas canvas;

    public Text label;

    public string gunName = "Glock 19";

    public int price = 500;

    public float renderRange = 3f;

    /* State */

    private GunHolder gunHolder;

    private void Start()
    {
        SetTargetNearestPlayer();
    }

    private void Update()
    {
        if (IsWithinRenderRange())
        {
            Economy economy = gunHolder.GetComponent<Economy>();

            SetText(gunName + " | " + price);

            label.color = economy.ContainsAtleast(price) ? Color.white : Color.red;

            canvas.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (economy.Transaction(-price))
                {
                    gunHolder.AddGun(gunName);

                    gunHolder.hudController.UpdateMoney(economy.balance, -price);
                }
            }

        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    public void SetText(string text)
    {
        label.text = text;
    }

    private void SetTargetNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject closest = SearchUtil.FindClosest(players, transform.position);

        gunHolder = closest.GetComponent<GunHolder>();
    }

    private bool IsWithinRenderRange()
    {
        if (gunHolder == null)
            return false;

        // Calculate square distance
        float sqrDistance = (gunHolder.transform.position - transform.position).sqrMagnitude;

        return sqrDistance < renderRange * renderRange;
    }
}
