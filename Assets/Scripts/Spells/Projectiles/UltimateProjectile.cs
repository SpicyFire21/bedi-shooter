using UnityEngine;

public abstract class UltimateProjectile : ProjectileSpellBase
{

    public float cinematicDuration;


    public abstract void UltimateCinematic();

    public abstract void ShakeCamera(float intensity);
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
        Vector3 position = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitMask))
        {
            float dist = Vector3.Distance(caster.transform.position, hit.point);
            if (dist <= 10)
            {
                position = hit.point;
                return position;
            }

            // empêche le joueur de placer la zone plus loin que la portée maximum (maxRange) --> clamp
            Vector3 dir = (hit.point - caster.transform.position).normalized;
            position = caster.transform.position + dir * 10f;
            return position;
        }

        return position;
    }

}
