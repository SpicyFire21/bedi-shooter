using UnityEngine;

public abstract class UltimateProjectile : ProjectileSpellBase
{

    public float cinematicDuration;
    public float cameraShakeIntensity;
    public float cameraShakingDuration;


    public abstract void UltimateCinematic();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetCinematicCastPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f, hitMask);

        Vector3 origin = caster.transform.position;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == caster.transform)
                continue;

            float dist = Vector3.Distance(origin, hit.point);
            if (dist <= 10f)
            {
                return hit.point;
            }
            else
            {
                Vector3 dir = (hit.point - origin).normalized;
                return origin + dir * 10f;
            }
        }

        Vector3 fallbackDir = ray.direction.normalized;
        return origin + fallbackDir * 10f;
    }


}
