using UnityEngine;
using System.Collections;

public class HollowPurple : UltimateProjectile
{
    public GameObject redPrefab;
    public GameObject bluePrefab;

    public float gapBetweenPrefabs; // ecart entre le rouge et le bleu
    public float fusionDuration;

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

        Vector3 direction = (castPosition - caster.transform.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;

        Vector3 redPosition = castPosition - right * gapBetweenPrefabs;
        Vector3 bluePosition = castPosition + right * gapBetweenPrefabs;

        redPosition.y = caster.transform.position.y + 1f;
        bluePosition.y = caster.transform.position.y + 1f;

        GameObject red = Instantiate(redPrefab, redPosition, Quaternion.identity);
        GameObject blue = Instantiate(bluePrefab, bluePosition, Quaternion.identity);

        // pour le deplacement des prefabs
        caster.StartCoroutine(FusionCoroutine(red.transform, blue.transform, castPosition));
    }

    private IEnumerator FusionCoroutine(Transform red, Transform blue, Vector3 center)
    {
        float elapsed = 0f;

        Vector3 redStart = red.position;
        Vector3 blueStart = blue.position;

        // hauteur final
        center.y = red.position.y;

        while (elapsed < fusionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fusionDuration;

            // on va vers le centre
            red.position = Vector3.Lerp(redStart, center, t);
            blue.position = Vector3.Lerp(blueStart, center, t);

            yield return null;
        }

        red.position = center;
        blue.position = center;
        Destroy(red.gameObject);
        Destroy(blue.gameObject);
    }

}
