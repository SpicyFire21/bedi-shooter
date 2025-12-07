using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [Header("References")]
    [SerializeField] private Player player;

    [Header("Current Equipped Items")]
    private ItemData equippedHead;
    private ItemData equippedChest;
    private ItemData equippedLegs;
    private ItemData equippedWeapon;

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

    private void EquipHead(ItemData item, GameObject prefab)
    {
        equippedHead = item;
        Debug.Log("Équipé casque : " + item.name);
    }

    private void EquipChest(ItemData item, GameObject prefab)
    {
        equippedChest = item;
        Debug.Log("Équipé plastron : " + item.name);
    }

    private void EquipLegs(ItemData item, GameObject prefab)
    {
        equippedLegs = item;
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

        equippedWeapon = item;
        Debug.Log("Equipé : " + item.name);
    }
}