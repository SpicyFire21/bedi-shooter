using UnityEngine;
using System.Collections;


public abstract class RangeWeapon : Weapon
{
    [Header("Ammo")]

    public int maxAmmo;
    public int currentAmmo;
    public int magazineSize;
    private int ammoInMagazine;
    public float reloadTime;

    private bool isReloading = false;

    [Header("Bullet")]
    public float bulletSpeed;
    public GameObject bulletPrefab;

    [Header("Weapon stats")]
    public LayerMask enemyMask;

    [Header("Sound")]
    public float soundCooldown;
    private float lastSoundTime;
    private float lastShotTime = 0f;
    public AudioClip reloadAudio;

    [HideInInspector] public Vector3 direction;

    // instance réelle équipée
    [HideInInspector] public ItemInstance equippedInstance;


    public override void Attack(Player player)
    {

        Debug.Log("last shottime : " + lastShotTime);
        // vérifie le cooldown --> pour 4 d'attack speed --> 4 balles par secondes
        if (Time.time - lastShotTime < 1f / weaponAttackSpeed)
            return;

        if (currentAmmo <= 0) return;

        direction = GetDirection(player);

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
        HandleAmmoInMagazine(player);

        if (equippedInstance != null)
        {
            WeaponRuntimeData runtime = Inventory.instance.GetWeaponRuntime(equippedInstance);
            runtime.currentAmmo = currentAmmo;
            runtime.lastSoundTime = lastSoundTime;
            runtime.ammoInMagazine = ammoInMagazine;
            runtime.lastShotTime = lastShotTime;
        }

        // mets à jour le dernier tir
        lastShotTime = Time.time;
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

        // si c'est la première fois qu'on initialise l'arme
        if (!runtime.initialized)
        {
            currentAmmo = maxAmmo;     
            ammoInMagazine = magazineSize; 
            runtime.initialized = true;
            isReloading = false;
            lastShotTime = 0f;
            lastSoundTime = 0f;
        }
        else
        {
            currentAmmo = runtime.currentAmmo;
            ammoInMagazine = runtime.ammoInMagazine;
            lastSoundTime = 0f;
        }
    }


    private void HandleAmmoInMagazine(Player player)
    {
        ammoInMagazine--;
        if (ammoInMagazine == 0 && currentAmmo > magazineSize)
        {
            StartReload(player);
        }
        else if (currentAmmo < magazineSize)
        {
            ammoInMagazine = magazineSize;
        } else if (currentAmmo == 0)
        {
            ammoInMagazine = 0;
        }
    }

    public void StartReload(Player player)
    {
        if (!isReloading && ammoInMagazine < magazineSize && currentAmmo > 0)
        {
            player.StartCoroutine(ReloadCoroutine(player));
        }
    }

    private IEnumerator ReloadCoroutine(Player player)
    {
        isReloading = true;

        Debug.Log("Reloading...");
        AudioSource.PlayClipAtPoint(reloadAudio, player.transform.position, 2f);
        yield return new WaitForSeconds(reloadTime);
        int neededAmmo = magazineSize - ammoInMagazine;
        int ammoToLoad = Mathf.Min(neededAmmo, currentAmmo);
        

        ammoInMagazine += ammoToLoad;
        //currentAmmo -= ammoToLoad;

        isReloading = false;
        Debug.Log("Reload complete");
    }

    public int getAmmoInMagazine()
    {
        return ammoInMagazine;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public float GetLastShotTime()
    {
        return lastShotTime;
    }
}
