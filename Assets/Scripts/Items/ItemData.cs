using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public ItemType itemType;
    public int itemLevel;
}

public enum ItemType
{
    Weapon,
    Consumable,
}


