using UnityEngine;

public abstract class UltimateMeleeWeapon : MeleeWeapon
{
    public float maxRightClickDistance;
    public float rightClickCooldown;
    public float abilityCooldown;

    protected float nextRightClickTime = 0f;
    protected float nextAbilityTime = 0f;
    public float specialAbilityDamage = 200f;
    public LayerMask canRightClickMask;
    public string rightClickAbilityName, specialAbilityName;
    public AudioClip rightClickAbilitySound, specialAbilitySound;
    [HideInInspector] public ItemInstance equippedInstance;

    public abstract void DoRightClick(Player player);
    public abstract void SpecialAbility(Player player);

    public abstract void ImpactEffect(Player player, Monster monster);

    public bool CanRightClick()
    {
        return Time.time >= nextRightClickTime;
    }

    public bool CanCastAbility()
    {
        return Time.time >= nextAbilityTime;
    }

    public float GetCurrentAbilityCooldown()
    {
        if (nextAbilityTime - Time.time >= 0)
        {
            return nextAbilityTime - Time.time;
        }
        else
        {
            return 0f;
        }
    }

    public float GetCurrentRightClickCooldown()
    {
        if (nextRightClickTime - Time.time >= 0)
        {
            return nextRightClickTime - Time.time;
        } else
        {
            return 0f;
        }
    }

    protected void TriggerRightClickCooldown()
    {
        nextRightClickTime = Time.time + rightClickCooldown;
    }

    protected void TriggerAbilityCooldown()
    {
        nextAbilityTime = Time.time + abilityCooldown;
    }
    public Vector3 GetRightClickPosition(Player player, float maxDistance)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, canRightClickMask);

        Vector3 origin = player.transform.position;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == player.transform)
                continue;

            float dist = Vector3.Distance(origin, hit.point);
            if (dist <= maxDistance)
            {
                return hit.point;
            }
            else
            {
                Vector3 dir = (hit.point - origin).normalized;
                
                return origin + dir * maxDistance;
            }
        }

        Vector3 fallbackDir = ray.direction.normalized;
        return origin + fallbackDir * maxDistance;
    }

    public void Initialize(ItemInstance instance)
    {
        equippedInstance = instance;
        UltimateMeleeWeaponRuntimeData runtime = Inventory.instance.GetUltimateMeleeWeaponRuntime(instance);

        // si c'est la première fois qu'on initialise l'arme
        if (!runtime.initialized)
        {
            nextRightClickTime = 0f;
            nextAbilityTime = 0f;

            runtime.initialized = true;
        }
        else
        {
            runtime.maxRightClickDistance = maxRightClickDistance;
            runtime.rightClickCooldown = rightClickCooldown;
            runtime.canRightClickMask = canRightClickMask;
            nextRightClickTime = 0f;
            nextAbilityTime = 0f;

        }
    }

    public float GetNextRightClickTime()
    {
        return nextRightClickTime;
    }


}

