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

    [Header("Audio")]
    public AudioClip deathSound;    // son joué quand le sort est lancé
    public bool soundDeadAlreadyDone = false;


    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void DeathHandler(float destructionTime)
    {
        Destroy(gameObject, destructionTime);
        if (!soundDeadAlreadyDone)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, 1f);
            soundDeadAlreadyDone = true;
        }
        anim.SetBool("Is_dead", true);
        agent.isStopped = true;
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
