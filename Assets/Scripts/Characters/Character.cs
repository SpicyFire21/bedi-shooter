using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    public float currentHealth;
    public string characterName;

    [Header("Movement")]
    public float moveSpeed;
    public Transform castPoint;
    public bool isPlayer; // false par défaut
    public bool isDead;
    public bool deathSoundPlayed = false;


    [Header("Attacks")]
    public abstract float MeleeRange { get; }
    public abstract float MeleeRadius { get; }
    public abstract int MeleeDamage { get; }
    public abstract LayerMask MeleeMask { get; }


    protected void HandleDeath()
    {
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            DeathHandler(3f);
        }
    }

    public abstract void DeathHandler(float destructionTime);

    public void TakeDamage(float damage)
    {
        float newHealth = currentHealth -= damage;
        currentHealth = newHealth;
        if (this is Monster monster)
        {
            monster.UpdateHealthBar();
        }
    }

    public void TakeHealth(float health)
    {
        float newHealth = currentHealth += health;
        if (newHealth < maxHealth)
        {
            Debug.Log("on peut ! : " + newHealth);
            currentHealth = newHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }
        if (this is Monster monster)
        {
            monster.UpdateHealthBar();
        }
    }

    public abstract void Attack();

}