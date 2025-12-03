using System.Collections.Generic;
using UnityEngine;

public class ZoneSpellBase : SpellBase
{
    [Header("Zone Settings")]
    [SerializeField] protected float maxRange = 20f;
    [SerializeField] protected LayerMask groundLayer;

    protected bool isActiveZone = false;
    protected HashSet<Character> targets = new HashSet<Character>();

    protected override void Cast()
    {
        if (data == null || data.prefab == null)
        {
            Debug.LogError("Données de sort ou prefab manquants.");
            return;
        }

        if (!TryGetTargetPosition(out Vector3 spawnPos))
        {
            Debug.LogWarning("Aucun point valide sous la souris pour placer la zone.");
            return;
        }

        GameObject instance = Instantiate(data.prefab, spawnPos, Quaternion.identity);

        ZoneSpellBase zone = instance.GetComponent<ZoneSpellBase>();
        zone.Initialize(caster, data);
        // la zone gère sa propre destruction si besoin
    }

    public virtual void Initialize(Character caster, SpellData data)
    {
        data.lifeTime = data.lifeTime + 0.1f;
        this.caster = caster;
        this.data = data;
        isActiveZone = true;
        Destroy(gameObject, data.lifeTime);
    }

    protected bool TryGetTargetPosition(out Vector3 position)
    {
        position = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            float dist = Vector3.Distance(caster.transform.position, hit.point);
            if (dist <= maxRange)
            {
                position = hit.point;
                return true;
            }

            // empêche le joueur de placer la zone plus loin que la portée maximum (maxRange) --> clamp
            Vector3 dir = (hit.point - caster.transform.position).normalized;
            position = caster.transform.position + dir * maxRange;
            return true;
        }

        return false;
    }
}
