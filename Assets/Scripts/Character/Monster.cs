using UnityEngine;

public class Monster : Character
{
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject, 3f);
    }
}
