using UnityEngine;

public class BaseGun : RangeWeapon
{
    public override void DoAnimation(Player player)
    {
        /*
        player.animator.SetFloat("AttackSpeed", weaponAttackSpeed);
        player.animator.SetTrigger("BaseAttack02");
        player.tps.canMove = false;
        AudioSource.PlayClipAtPoint(actionSound, transform.position, 1f);
        */
        player.animator.SetTrigger("WeaponIdleAnimation");
    }
}
