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

    [Header("Base stats")]
    public float baseMaxHealth;
    public float baseMoveSpeed = 2.5f;


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
        } else if (this is Player player)
        {
                Animator animator = player.animator;

                // animation deja occupé --> on degage
                if (animator.IsInTransition(0))
                    return;

                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

                // si une anim est deja en cours on va preferer la faire --> idle cest les actions de base comme courir, bouger...
                if (!state.IsTag("Idle"))
                    return;

                animator.SetTrigger("IsTakingDamage");
        }
    }

    public void TakeHealth(float health)
    {
        float newHealth = currentHealth += health;
        if (newHealth < maxHealth)
        {
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

}