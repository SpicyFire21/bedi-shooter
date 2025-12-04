using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public GameObject itemPrefab;
    public Sprite itemSprite;
    public ItemType itemType;
}

public enum ItemType
{
    Weapon,
    Potion,
    Consumable,
    Quest
}


