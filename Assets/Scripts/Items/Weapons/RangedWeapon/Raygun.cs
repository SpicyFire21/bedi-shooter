
using UnityEngine;
public class Raygun : RangeWeapon
{
    public override void DoAnimation(Player player)
    {
        if (!player.animator.GetBool("WeaponIdleAnimation"))
        {
            player.animator.SetBool("WeaponIdleAnimation", true);
        }
    }

    public override void StopAnimation(Player player)
    {
        if (player.animator.GetBool("WeaponIdleAnimation"))
        {
            player.animator.SetBool("WeaponIdleAnimation", false);
        }
    }
}
