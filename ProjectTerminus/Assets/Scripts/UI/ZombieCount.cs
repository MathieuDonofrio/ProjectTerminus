using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieCount : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Zombie count label")]
    public Text label;

    /* Services */

    public void UpdateCount(int count)
    {
        if(label.text != count.ToString())
        {
            label.text = count.ToString();
        }
    }
}
