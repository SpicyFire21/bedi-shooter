using UnityEngine;
using System.Collections;

public class HollowPurple : UltimateProjectile
{
    public GameObject redPrefab;
    public GameObject bluePrefab;

    public float offsetDistance; // réduit pour être proche du joueur
    public float forwardOffset;
    public float fusionDuration;
    public float cameraShakeIntensity;

    private void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();

        if (target != null && target != caster)
        {
            if (target.currentHealth <= 0) return;
            target.TakeDamage(localValue);
        }
    }

    public override void UltimateCinematic()
    {
        HollowPurpleCinematic();
    }

    private void HollowPurpleCinematic()
    {
        Player player = caster as Player;
        Vector3 castPosition = GetCinematicCastPosition();
        Debug.Log("cast pos : " + castPosition);


        Vector3 redPosition = castPosition + new Vector3(castPosition.x + offsetDistance, castPosition.y, castPosition.z); // à gauche
        Vector3 bluePosition = castPosition + new Vector3(castPosition.x - offsetDistance, castPosition.y, castPosition.z); // à droite

        Debug.Log("redposition : " + redPosition);
  
        Instantiate(redPrefab, redPosition, Quaternion.identity); 
        Instantiate(bluePrefab, bluePosition, Quaternion.identity);
    }

    public override void ShakeCamera(float intensity)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        cam.transform.position = Random.insideUnitSphere * intensity;
    }
}
