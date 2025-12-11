using UnityEngine;
using System.Collections;

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
            return;

        UltimateProjectile ultimatePrefab = data.prefab.GetComponent<UltimateProjectile>();

        if (ultimatePrefab != null)
        {
            caster.StartCoroutine(UltimateCoroutine(ultimatePrefab));
        }
        else
        {
            CastNormalProjectile();
        }
    }

    private void CastNormalProjectile()
    {
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

        Player playerCaster = caster as Player;
        float levelBonusMultiplier = 1f;
        if (playerCaster != null)
        {
            levelBonusMultiplier = 1f + (0.05f * (playerCaster.level - 1));
        }

        localValue = data.value * levelBonusMultiplier;

        Destroy(gameObject, data.lifeTime);
    }

    protected virtual Vector3 GetDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitMask))
        {
            return (hit.point - caster.castPoint.position).normalized;
        }

        return ray.direction;
    }

    private IEnumerator UltimateCoroutine(UltimateProjectile ultimatePrefab)
    {
        Player player = caster as Player;
        player.tps.enabled = false;
        player.tps.canMove = false;
        ultimatePrefab.UltimateCinematic();
        Debug.Log("CINEMATIQUE");
        yield return new WaitForSeconds(ultimatePrefab.cinematicDuration);
        player.tps.enabled = true;
        player.tps.canMove = true;
        CameraShake.Instance.Shake(ultimatePrefab.cameraShakeIntensity, ultimatePrefab.cameraShakingDuration);
        CastNormalProjectile();
    }
}
