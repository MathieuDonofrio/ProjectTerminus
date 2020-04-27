using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Starting balance at begining of game")]
    public int startingBalance = 0;

    /* State */

    public int balance { get; private set; }

    private void Start()
    {
        balance = startingBalance;
    }

    /* Services */

    public bool Transaction(int amount)
    {
        bool success = true;

        if (amount < 0)
        {
            if (ContainsAtleast(Mathf.Abs(amount))) balance += amount;
            else success = false;
        }
        else if (amount != 0) balance += amount;

        return success;
    }

    public bool ContainsAtleast(int amount)
    {
        return balance >= amount;
    }
}
