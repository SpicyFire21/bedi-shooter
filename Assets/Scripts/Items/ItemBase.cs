using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public ItemData data;
    private bool pickedUp = false;

    public virtual void OnPickup()
    {
            Inventory.instance.AddItem(data); 
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player") && CompareTag("Pickable"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                pickedUp = true;
                OnPickup();
            }
        }
    }

}
