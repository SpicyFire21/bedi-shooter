using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    [Header("References")]
    [SerializeField] private Player player;

    private Dictionary<Transform, Mesh> originalMeshes = new Dictionary<Transform, Mesh>();
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();

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

            case EquipmentSlot.Feet:
                EquipBoots(item, prefab);
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
                UnequipHead();
                break;

            case EquipmentSlot.Chest:
                UnequipChest();
                break;

            case EquipmentSlot.Legs:
                UnequipLegs();
                break;

            case EquipmentSlot.Hands:
                UnequipWeapon();
                break;
            case EquipmentSlot.Feet:
                UnequipBoots();
                break;
        }
    }

    private void EquipHead(ItemData item, GameObject prefab)
    {
        EquipOnSlot(player.headSlot, item, prefab);
    }

    private void EquipChest(ItemData item, GameObject prefab)
    {
        EquipOnSlot(player.chestSlot, item, prefab);
    }

    private void EquipLegs(ItemData item, GameObject prefab)
    {

    }

    private void EquipBoots(ItemData item, GameObject prefab)
    {

    }

    private void EquipWeapon(ItemData item, GameObject prefab)
    {
        EquipOnSlot(player.weaponSlot, item, prefab);

        Weapon weapon = prefab.GetComponent<Weapon>();
        if (weapon == null)
            weapon = prefab.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            player.SetEquippedWeapon(weapon);
        }
    }



    private void UnequipHead()
    {
        UnequipFromSlot(player.headSlot, EquipmentSlot.Head);
    }

    private void UnequipChest()
    {
        UnequipFromSlot(player.chestSlot, EquipmentSlot.Chest);
    }

    private void UnequipLegs()
    {

    }

    private void UnequipWeapon()
    {
        UnequipFromSlot(player.weaponSlot, EquipmentSlot.Hands);
        player.SetEquippedWeapon(null);
    }

    private void UnequipBoots()
    {

    }


    private void EquipOnSlot(Transform slot, ItemData item, GameObject prefab)
    {
        if (slot == null || prefab == null || item == null) return;

        SkinnedMeshRenderer slotRenderer = slot.GetComponentInChildren<SkinnedMeshRenderer>();
        if (slotRenderer == null) return;

        // sauvegarde le mesh et les materiaux originaux si ce n'est pas déjà fait
        if (!originalMeshes.ContainsKey(slot))
            originalMeshes[slot] = slotRenderer.sharedMesh;
        if (!originalMaterials.ContainsKey(slot))
            originalMaterials[slot] = slotRenderer.sharedMaterial;

        // applique le mesh de l'item
        MeshFilter itemMeshFilter = prefab.GetComponentInChildren<MeshFilter>();
        if (itemMeshFilter != null)
            slotRenderer.sharedMesh = itemMeshFilter.sharedMesh;

        // applique le matériel de l'item
        Renderer itemRenderer = prefab.GetComponentInChildren<Renderer>();
        if (itemRenderer != null)
            slotRenderer.material = itemRenderer.sharedMaterial;
    }

    private void UnequipFromSlot(Transform slot, EquipmentSlot equipmentSlot)
    {
        if (slot == null) return;

        SkinnedMeshRenderer slotRenderer = slot.GetComponentInChildren<SkinnedMeshRenderer>();
        if (slotRenderer == null) return;

        // remet le mesh et le matériel originaux
        if (originalMeshes.TryGetValue(slot, out Mesh originalMesh))
            slotRenderer.sharedMesh = originalMesh;
        else
            slotRenderer.sharedMesh = null;

        if (originalMaterials.TryGetValue(slot, out Material originalMat))
            slotRenderer.material = originalMat;
    }


}