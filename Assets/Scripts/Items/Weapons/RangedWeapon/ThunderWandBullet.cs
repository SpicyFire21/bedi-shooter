using UnityEngine;

public class ThunderWandBullet : Bullet
{

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("salut ");
        if (((1 << other.gameObject.layer) & enemyMask) == 0) return; // pour résumer cette ligne, si cest pas un ennemi on return
        Monster target = other.GetComponent<Monster>();

        if (target == null || target == player) return;
        if (target.isDead) return;
        player.SetAttackTarget(target);
        player.ApplyWeaponDamage();
    }
}
