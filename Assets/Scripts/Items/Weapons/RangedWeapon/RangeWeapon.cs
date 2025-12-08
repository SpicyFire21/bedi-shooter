using UnityEngine;

public class RangeWeapon : Weapon
{
    public override void Attack(Player player)
    {

    }

    public override Monster GetTarget(float maxDistance, Player player)
    {
        return null;
    }
}
