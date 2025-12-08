using UnityEngine;


public struct ShootHitInfo
{
    public Vector3 direction;
    public Vector3 hitPoint;
    public Transform target;
}

public abstract class RangeWeapon : Weapon
{

    public int currentAmmo;
    public int maxAmmo;
    public int magazineSize;
    public GameObject projectilePrefab;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public override void Attack(Player player)
    {
        if (currentAmmo <= 0) return;

        DoAnimation(player);

        ShootHitInfo shootInfo = GetShootInfo(weaponRange, player);

        // Exemple : instanciation du projectile
        GameObject proj = Instantiate(projectilePrefab, player.castPoint.position, Quaternion.identity);

        proj.transform.forward = shootInfo.direction;

        // Si tu as touché une target
        if (shootInfo.target != null)
        {
            Debug.Log("Cible touchée : " + shootInfo.target.name);
        }

        currentAmmo--;
    }


    public ShootHitInfo GetShootInfo(float maxDistance, Player player)
    {
        ShootHitInfo info = new ShootHitInfo();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, weaponRange, hitMask))
        {
            info.direction = (hit.point - player.castPoint.position).normalized;
            info.hitPoint = hit.point;
            info.target = hit.transform;
        }
        else
        {
            info.direction = ray.direction;
            info.hitPoint = ray.origin + ray.direction * maxDistance;
            info.target = null;
        }

        return info;
    }



    public abstract void DoAnimation(Player player);
}
