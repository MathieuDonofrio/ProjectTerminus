using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyParticle : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Duration of the particle")]
    public float duration = 1;

    [Tooltip("Speed of the particle")]
    public float speed = 10;

    [Tooltip("Color of effect when gaining money")]
    public Color gainMoneyColor;

    [Tooltip("Color of effect when losing money")]
    public Color loseMoneyColor;

    [Tooltip("Amount of money displayed in the particle")]
    public Text moneyText;

    /* State */

    private MoneyParticleSystem moneyParticleSystem;

    private Vector3 velocity;

    private float lastStart;

    private void Update()
    {
        if(Time.time - lastStart <= duration)
        {
            transform.position += velocity * Time.deltaTime;
        }
        else
        {
            Kill();
        }
    }

    /* Services */

    public void StartParticle(MoneyParticleSystem moneyParticleSystem, int amount)
    {
        this.moneyParticleSystem = moneyParticleSystem;

        float y = Random.value;

        if(amount > 0)
        {
            moneyText.text = "+" + amount;

            moneyText.color = gainMoneyColor;
        }
        else
        {
            moneyText.text = amount.ToString();

            moneyText.color = loseMoneyColor;

            y *= -1;
        }

        velocity = new Vector2(1.5f, y).normalized * speed;

        lastStart = Time.time;
    }

    public void Kill()
    {
        if(moneyParticleSystem == null)
        {
            Destroy(this);
        }
        else
        {
            moneyParticleSystem.CheckIn(this);
        }
    }

}
