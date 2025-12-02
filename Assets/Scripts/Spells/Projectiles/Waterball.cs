using UnityEngine;

public class Waterball : ProjectileSpellBase
{
    private void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();

        if (target != null && target != caster)
        {
            target.TakeDamage(data.value);
            Destroy(gameObject);
        }

        if (other.CompareTag("Destructible"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
