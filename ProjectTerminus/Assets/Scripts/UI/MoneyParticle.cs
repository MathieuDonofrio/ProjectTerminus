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

    public MoneyParticleSystem MoneyParticleSystem { get; set; }

    private float lastStart;

    private Vector2 velocity;

    private void Update()
    {
        if(Time.time - lastStart <= duration)
        {
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            Finished();
        }
    }

    /* Services */

    public void StartParticle(Vector2 position, int amount)
    {
        transform.position = position;

        float y = 0;

        if(amount > 0)
        {
            moneyText.text = "+" + amount;

            moneyText.color = gainMoneyColor;

            y = Random.value;
        }
        else
        {
            moneyText.text = amount.ToString();

            moneyText.color = loseMoneyColor;

            y = -Random.value;
        }

        velocity = new Vector2(1.5f, y).normalized * speed;

        lastStart = Time.time;
    }

    public void Finished()
    {
        if(MoneyParticleSystem != null)
        {
            MoneyParticleSystem.CheckIn(this);
        }
    }

}
