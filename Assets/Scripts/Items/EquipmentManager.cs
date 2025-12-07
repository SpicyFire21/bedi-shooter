using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [Header("References")]
    [SerializeField] private Player player;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Equip(ItemData item, GameObject prefab)
    {
        switch (item.EquipmenttSlot)
        {
            case EquipmentSlot.Head:
                EquipHead(item, prefab);
                break;

            case EquipmentSlot.Chest:
                EquipChest(item, prefab);
                break;

            case EquipmentSlot.Legs:
                EquipLegs(item, prefab);
                break;

            case EquipmentSlot.Hands:
                EquipWeapon(item, prefab);
                break;

            default:
                Debug.LogWarning("Slot non géré : " + item.EquipmenttSlot);
                break;
        }
    }


    public void Unequip(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Head:
                Debug.Log("Casque retiré");
                break;

            case EquipmentSlot.Chest:
                Debug.Log("Plastron retiré");
                break;

            case EquipmentSlot.Legs:
                Debug.Log("Jambes retirées");
                break;

            case EquipmentSlot.Hands:
                UnequipWeapon();
                Debug.Log("Arme retirée");
                break;
        }
    }

    private void EquipHead(ItemData item, GameObject prefab)
    {
        Debug.Log("Équipé casque : " + item.name);
    }

    private void EquipChest(ItemData item, GameObject prefab)
    {
        Debug.Log("Équipé plastron : " + item.name);
    }

    private void EquipLegs(ItemData item, GameObject prefab)
    {
        Debug.Log("Équipé jambes : " + item.name);
    }

    private void EquipWeapon(ItemData item, GameObject prefab)
    {
        if (prefab == null || item == null) return;

        SkinnedMeshRenderer slotRenderer = player.weaponSlot?.GetComponentInChildren<SkinnedMeshRenderer>();
        if (slotRenderer == null) return;

        MeshFilter itemMeshFilter = prefab.GetComponentInChildren<MeshFilter>();
        if (itemMeshFilter != null)
            slotRenderer.sharedMesh = itemMeshFilter.sharedMesh;

        Renderer itemRenderer = prefab.GetComponentInChildren<Renderer>();
        if (itemRenderer != null)
            slotRenderer.material = itemRenderer.sharedMaterial;
        Debug.Log("Equipé : " + item.name);
    }
    private void UnequipWeapon()
    {
        SkinnedMeshRenderer slotRenderer = player.weaponSlot?.GetComponentInChildren<SkinnedMeshRenderer>();
        if (slotRenderer == null) return;

        slotRenderer.sharedMesh = null;
    }
}