using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected Vector3 startPosition;
    protected float maxDistance;
    protected Player player;
    protected float damage;
    protected LayerMask enemyMask;

    public void Init(float range, float bulletDamage, LayerMask layerMask, Player caster)
    {
        startPosition = transform.position;
        maxDistance = range;
        damage = bulletDamage;
        enemyMask = layerMask;
        player = caster;
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyMask) == 0) return; // pour résumer cette ligne, si cest pas un ennemi on return
        Monster target = other.GetComponent<Monster>();

        if (target == null || target == player) return;
        if (target.isDead) return;
        player.SetAttackTarget(target);
        player.ApplyWeaponDamage();
        Destroy(gameObject);
    }
}
