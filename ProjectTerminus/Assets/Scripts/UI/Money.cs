using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    /* Configuration */

    [Tooltip("How long the scaling animation should last ")]
    public float animationDuration = 0.15f;

    [Tooltip("Text that contains balance")]
    public Text balanceText;

    [Tooltip("Money particle system used to spawn money particles")]
    public MoneyParticleSystem moneyParticleSystem;

    /* State */

    private float lastParticleTime;

    private void Update()
    {
        float elapsed = Time.time - lastParticleTime;

        if(elapsed <= animationDuration)
        {
            float scale = 1 + 0.4f * Mathf.Cos(Mathf.Clamp01(elapsed / animationDuration));

            balanceText.transform.localScale = Vector2.one * scale;
        }
    }

    /* Services */

    public void UpdateBalance(int balance)
    {
        balanceText.text = balance.ToString();
    }

    public void SpawnParticle(int amount)
    {
        moneyParticleSystem.SpawnParticle(transform.position, amount);

        lastParticleTime = Time.time;
    }
}
