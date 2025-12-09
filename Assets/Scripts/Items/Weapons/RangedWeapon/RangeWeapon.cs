using UnityEngine;

public abstract class RangeWeapon : Weapon
{
    public int maxAmmo;
    public int currentAmmo;
    public int magazineSize;
    public float bulletSpeed;
    public GameObject bulletPrefab;

    public RaycastHit rayHit;
    public Vector3 direction;
    public LayerMask enemyMask;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    public override void Attack(Player player)
    {
        if (currentAmmo <= 0) return;

        direction = GetDirection(player);

        GameObject instance = Instantiate(
            bulletPrefab,
            player.castPoint.position,
            bulletPrefab.transform.rotation
        );

        instance.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody bulletRb = instance.GetComponent<Rigidbody>();
        if (bulletRb == null) return;

        bulletRb.linearVelocity = direction * bulletSpeed; 

        Bullet bullet = instance.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(weaponRange, weaponDamage, enemyMask, player);

        currentAmmo--;
    }



    public Vector3 GetDirection(Player player)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, weaponRange, hitMask))
        {
            return (hit.point - player.castPoint.position).normalized;
        }

        return ray.direction.normalized;
    }

    //public Monster GetTarget(Player player)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, weaponRange, enemyMask))
    //    {
    //        if (hit.collider.TryGetComponent<Monster>(out Monster character))
    //        {
    //            return character; 
    //        }
    //    }

    //    return null;
    //}


    public abstract void DoAnimation(Player player);
}
