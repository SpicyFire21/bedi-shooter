using UnityEngine;
using UnityEngine.AI;

public class Monster : Character
{
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    [Header("AI Settings")]
    public Transform player;

    public NavMeshAgent agent;
    public Animator anim;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public float lastAttackTime;


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
