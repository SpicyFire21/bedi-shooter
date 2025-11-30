using UnityEngine;

public class ManaPickUp : MonoBehaviour
{
    public float manaAmount = 5f;
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        // on vérifie si c'est un joueur ou un personnage
        Player player = other.GetComponent<Player>();

        if (player == null) return;

        player.currentMana += manaAmount;

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
