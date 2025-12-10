using UnityEngine;

public class HollowPurple : ProjectileSpellBase
{
    private void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();

        if (target != null && target != caster)
        {
            if (target.currentHealth <= 0) return;
            target.TakeDamage(localValue);
        }
    }
}
