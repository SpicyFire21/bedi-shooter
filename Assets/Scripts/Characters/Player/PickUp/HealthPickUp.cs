using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 50f;
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        // on vérifie si c'est un joueur ou un personnage
        Player player = other.GetComponent<Player>();

        if (player == null) return;

        player.currentHealth += healAmount;

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
