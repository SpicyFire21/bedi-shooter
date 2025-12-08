using UnityEngine;

public abstract class Equipment : ItemBase
{
    public float equipmentHealthBonus;
    public float equipmentDamageBonus;
    public float equipmentSpeedBonus;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ApplyEquipmentStats(Player player)
    {
        player.damage += equipmentDamageBonus;
        player.maxHealth += equipmentHealthBonus;
        player.moveSpeed += equipmentSpeedBonus;
    }
}
