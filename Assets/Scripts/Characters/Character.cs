using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    public float currentHealth;
    public string characterName;

    [Header("Movement")]
    public float moveSpeed;

    public bool isPlayer; // false par défaut
    public bool isDead;
    public bool deathSoundPlayed = false;


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
        currentHealth -= damage;
        if (this is Monster monster)
        {
            monster.UpdateHealthBar();
        }
    }
}
