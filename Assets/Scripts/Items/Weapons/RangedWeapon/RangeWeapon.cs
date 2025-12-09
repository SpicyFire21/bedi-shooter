using UnityEngine;

public abstract class RangeWeapon : Weapon
{
    [Header("Ammo")]
    public int maxAmmo;
    public int currentAmmo;
    public int magazineSize;

    [Header("Bullet")]
    public float bulletSpeed;
    public GameObject bulletPrefab;

    [Header("Weapon stats")]
    public LayerMask enemyMask;

    [Header("Sound")]
    public float soundCooldown;
    private float lastSoundTime;

    [HideInInspector] public Vector3 direction;

    // ✅ Instance réelle équipée
    [HideInInspector] public ItemInstance equippedInstance;

    public override void Attack(Player player)
    {
        if (currentAmmo <= 0) return;

        // direction tir
        direction = GetDirection(player);

        // projectile
        GameObject instanceGO = Instantiate(
            bulletPrefab,
            player.castPoint.position,
            bulletPrefab.transform.rotation
        );

        instanceGO.transform.rotation = Quaternion.LookRotation(direction);

        Rigidbody bulletRb = instanceGO.GetComponent<Rigidbody>();
        if (bulletRb != null)
            bulletRb.linearVelocity = direction * bulletSpeed;

        Bullet bullet = instanceGO.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Init(weaponRange, weaponDamage, enemyMask, player);
        if (Time.time - lastSoundTime >= soundCooldown)
        {
            AudioSource.PlayClipAtPoint(actionSound, player.transform.position, 1f);
            lastSoundTime = Time.time;
        }
        currentAmmo--;
        if (equippedInstance != null)
        {
            
            WeaponRuntimeData runtime = Inventory.instance.GetWeaponRuntime(equippedInstance);
            runtime.currentAmmo = currentAmmo;
            runtime.lastSoundTime = lastSoundTime;
            Debug.Log("ammo : " + runtime.currentAmmo);
        }
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

    public float GetLastSoundTime()
    {
        return lastSoundTime;
    }

    public abstract void DoAnimation(Player player);

    public void Initialize(ItemInstance instance)
    {
        equippedInstance = instance;
        WeaponRuntimeData runtime = Inventory.instance.GetWeaponRuntime(instance);
        currentAmmo = runtime.currentAmmo;
        lastSoundTime = runtime.lastSoundTime;
    }
}
