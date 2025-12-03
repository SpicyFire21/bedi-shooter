using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileSpellBase : SpellBase
{
    [Header("Projectile Settings")]
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected LayerMask hitMask;

    protected Rigidbody rb;
    protected Vector3 direction;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Cast()
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("Données de sort ou prefab manquants.");
            return;
        }

        GameObject instance = Instantiate(
            data.prefab,
            caster.castPoint.position,
            Quaternion.identity
        );

        ProjectileSpellBase projectile = instance.GetComponent<ProjectileSpellBase>();

        if (projectile == null)
        {
            Debug.LogError("Le prefab ne contient pas de ProjectileSpellBase !");
            return;
        }

        projectile.Initialize(caster, data, GetDirection());
    }

 

    public virtual void Initialize(Character caster, SpellData data, Vector3 dir)
    {
        this.caster = caster;
        this.data = data;
        this.direction = dir;
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, data.lifeTime);
    }

    protected virtual Vector3 GetDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitMask)) // TOUT a part default, (le player)
        {
            return (hit.point - caster.castPoint.position).normalized;
        }

        return ray.direction;
    }
}
