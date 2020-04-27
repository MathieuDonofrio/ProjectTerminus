using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Text that contains balance")]
    public Text balanceText;

    [Tooltip("Money particle system used to spawn money particles")]
    public MoneyParticleSystem moneyParticleSystem;

    /* State */

    public void UpdateBalance(int balance)
    {
        balanceText.text = balance.ToString();
    }

    public void SpawnParticle(int amount)
    {
        moneyParticleSystem.SpawnParticle(transform.position, amount);
    }
}
