using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public ItemType itemType;
    public EquipmentSlot EquipmenttSlot;
    public int itemLevel;
}

public enum ItemType
{
    Consumable,
    Equipment
}

public enum EquipmentSlot
{
    Head,
    Chest,
    Hands,
    Legs,
    Feet,
}



