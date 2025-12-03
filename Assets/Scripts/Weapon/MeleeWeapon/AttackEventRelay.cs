using UnityEngine;

public class AttackEventRelay : MonoBehaviour
{
    public Weapon weapon;

    public void EnableDamage()
    {
        if (weapon != null)
            weapon.EnableDamage();
    }

    public void DisableDamage()
    {
        if (weapon != null)
            weapon.DisableDamage();
    }
}
