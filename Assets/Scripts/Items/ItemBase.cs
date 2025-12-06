using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public ItemData data;
    private bool pickedUp = false;

    public virtual void OnPickup(Player player)
    {
        if (player.inventory != null)
        {
            player.inventory.AddItem(data); 
            Destroy(gameObject);
            Debug.Log("Item ramassé : " + data.name);
        }
    }

    public abstract void Use(Player player);

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player") && CompareTag("Pickable"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log("On essaye d'add un item : " + data);
                pickedUp = true;
                OnPickup(player);
            }
        }
    }

}
