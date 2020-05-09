using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Current amount of health")]
    public float health;

    [Header("Head Settings")]
    [Tooltip("Eye height")]
    public float eyeHeight = 1.8f;

    [Tooltip("Head size")]
    public float headSize = 0.3f;

    [Header("Health Settings")]
    [Tooltip("Maximum amount of health")]
    public float maxHealth = 10f;

    [Header("Damage Settings")]
    [Tooltip("Invincibility makes all damage 0")]
    public bool invincible = false;

    [Tooltip("All damage taken is multiplied by this amount")]
    public float damageMultiplier = 1.0f;

    [Tooltip("All damage inflicted apawn self is multiplied by this amount")]
    public float selfDamageModifier = 0.5f;

    [Tooltip("All damage inflicted physically is multiplied by this amount")]
    public float physicalDamageModifier = 1.0f;

    [Tooltip("All damage inflicted by a projectile is multiplied by this amount")]
    public float projectileDamageModifier = 1.0f;

    [Tooltip("All damage inflicted by a explosion is multiplied by this amount")]
    public float explosionDamageModifier = 1.0f;

    /* Events */

    public UnityAction<float, GameObject, DamageType> onDamaged;

    public UnityAction<float, HealType> onHealed;

    public UnityAction onDeath;

    /* State */

    public bool IsDead { get; private set; }

    /* Other */

    private float lastHealth;

    private void Start()
    {
        health = maxHealth;
    }

    /* Services */

    public bool Damage(float damage, GameObject damageSource, DamageType type)
    {
        // Apply invincibility
        if (invincible) damage = 0.0f;

        // Apply self damage modifier
        if (gameObject == damageSource) damage *= selfDamageModifier;

        // Apply physical damage multiplier
        if (type == DamageType.PHYSICAL) damage *= physicalDamageModifier;

        // Apply projectile damage multiplier
        if (type == DamageType.PROJECTILE) damage *= projectileDamageModifier;

        // Apply explosion damage multiplier
        if (type == DamageType.EXPLOSION) damage *= explosionDamageModifier;

        // Apply damage multiplier
        damage *= damageMultiplier;

        // Record last health
        lastHealth = health;

        // Damage
        health = Mathf.Clamp(health - damage, 0f, maxHealth);

        // Get amount damaged
        damage = lastHealth - health;

        // Invoke event
        if (onDamaged != null)
        {
            onDamaged.Invoke(damage, damageSource, type);
        }

        // Kill
        if (health <= 0)
        {
            Kill();

            return true;
        }

        return false;
    }

    public void Heal(float heal, HealType type)
    {
        // Record last health
        lastHealth = health;

        // Heal
        health = Mathf.Clamp(health + heal, 0.0f, maxHealth);

        // Get amount healed
        heal = health - lastHealth;

        // Invoke event
        if (heal > 0f && onHealed != null)
        {
            onHealed.Invoke(heal, type);
        }
    }

    public void Kill()
    {
        if (IsDead)
            return;

        // Remove all health
        health = 0;

        // Set dead
        IsDead = true;

        // Invoke event
        if (onDeath != null)
        {
            onDeath.Invoke();
        }
    }

    public float HealthRatio()
    {
        return health / maxHealth;
    }

    public bool FullHealth()
    {
        return health >= maxHealth;
    }

}

public enum HealType
{
    CUSTOM, REGEN, PICKUP
}

public enum DamageType
{
    CUSTOM, PHYSICAL, PROJECTILE, EXPLOSION
}
