using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;
    public AudioClip equipmentEquipSound;

    [Header("References")]
    [SerializeField] private Player player;

    private Dictionary<Transform, Mesh> originalMeshes = new Dictionary<Transform, Mesh>();
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void Equip(ItemInstance instance, GameObject prefab)
    {
        AudioSource.PlayClipAtPoint(equipmentEquipSound, player.transform.position, 0.6f);
        switch (instance.data.EquipmentSlot)
        {
            case EquipmentSlot.Head:
                EquipHead(instance.data, prefab);
                break;

            case EquipmentSlot.Chest:
                EquipChest(instance.data, prefab);
                break;

            case EquipmentSlot.Legs:
                EquipLegs(instance.data, prefab);
                break;

            case EquipmentSlot.Hands:
                EquipWeapon(instance, prefab);
                break;

            case EquipmentSlot.Feet:
                EquipBoots(instance.data, prefab);
                break;

            default:
                Debug.LogWarning("Slot non géré : " + instance.data.EquipmentSlot);
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
        //EquipOnSlot(player.headSlot, item, prefab);
        Equipment equipment = prefab.GetComponent<Equipment>();
        if (equipment != null)
        {
            equipment.ApplyEquipmentStats(player);

        }
    }

    private void EquipChest(ItemData item, GameObject prefab)
    {
        //EquipOnSlot(player.chestSlot, item, prefab);
        Equipment equipment = prefab.GetComponent<Equipment>();
        if (equipment != null)
            equipment.ApplyEquipmentStats(player);
    }

    private void EquipLegs(ItemData item, GameObject prefab)
    {
        Equipment equipment = prefab.GetComponent<Equipment>();
        if (equipment != null)
            equipment.ApplyEquipmentStats(player);
    }

    private void EquipBoots(ItemData item, GameObject prefab)
    {
        Equipment equipment = prefab.GetComponent<Equipment>();
        if (equipment != null)
            equipment.ApplyEquipmentStats(player);
    }


    private void EquipWeapon(ItemInstance instance, GameObject prefab)
    {
        EquipOnSlot(player.weaponSlot, instance.data, prefab);

        Weapon weapon = prefab.GetComponent<Weapon>() ?? prefab.GetComponentInChildren<Weapon>();
        if (weapon == null) return;

        player.SetEquippedWeapon(weapon);

        RangeWeapon rangedWeapon = weapon as RangeWeapon;
        if (rangedWeapon != null)
        {
            rangedWeapon.Initialize(instance);
        }

        UltimateMeleeWeapon ultimateMeleeWeapon = weapon as UltimateMeleeWeapon;
        if (ultimateMeleeWeapon != null)
        {
            ultimateMeleeWeapon.Initialize(instance);
        }
    }






    private void UnequipHead()
    {
        RemoveEquipmentStats(Inventory.instance.getEquippedHelmet());
        //UnequipFromSlot(player.headSlot, EquipmentSlot.Head);
    }

    private void UnequipChest()
    {
        RemoveEquipmentStats(Inventory.instance.getEquippedChest());
        //UnequipFromSlot(player.chestSlot, EquipmentSlot.Chest);
    }

    private void UnequipLegs()
    {
        RemoveEquipmentStats(Inventory.instance.getEquippedLeggings());
    }

    private void UnequipWeapon()
    {
        UnequipFromSlot(player.weaponSlot, EquipmentSlot.Hands);
        player.SetEquippedWeapon(null);
    }

    private void UnequipBoots()
    {
        RemoveEquipmentStats(Inventory.instance.getEquippedBoots());
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

    private void RemoveEquipmentStats(ItemData itemData)
    {
        if (itemData == null) return;

        // on récupère le prefab de l'item équipé
        GameObject prefab = itemData.itemPrefab;
        if (prefab == null) return;

        // essaye de récupérer un composant Equipment sur le prefab
        Equipment equipment = prefab.GetComponent<Equipment>();
        if (equipment == null)
            equipment = prefab.GetComponentInChildren<Equipment>();

        if (equipment != null)
        {
            // supprime les stats appliquées au joueur
            equipment.UnapplyEquipmentStats(player);
        }
    }
}