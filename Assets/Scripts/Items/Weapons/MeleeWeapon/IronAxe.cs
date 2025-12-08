using UnityEngine;

public class IronAxe : MeleeWeapon
{
    public override void DoAnimation(Player player)
    {
        player.animator.SetFloat("AttackSpeed", weaponAttackSpeed);
        player.animator.SetTrigger("BaseAttack");
        player.tps.canMove = false; // peut etre a revoir car vraiment trop dur sinon de jouer melée
    }

   


    }
