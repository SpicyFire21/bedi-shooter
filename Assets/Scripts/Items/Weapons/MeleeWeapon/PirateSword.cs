using UnityEngine;

public class PirateSword : MeleeWeapon
{
    public override void DoAnimation(Player player)
    {
        player.animator.SetTrigger("BaseAttack");
    }
}
