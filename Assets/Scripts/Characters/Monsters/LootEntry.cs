using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public ItemData item;                     // l'item à drop
    [Range(0f, 1f)] public float dropChance;  // chance définie PAR le monstre
    public int minAmount = 1;
    public int maxAmount = 1;
}