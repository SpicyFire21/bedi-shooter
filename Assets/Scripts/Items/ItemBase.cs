using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public ItemData data;

    public virtual void OnPickup(Player player)
    {
        Debug.Log("Ramassé : " + data.itemName);
    }

    public abstract void Use(Player player);

    private void OnTriggerEnter(Collider other) // tout les items se ramassent en passant dessus
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            OnPickup(player);
        }
    }
}
