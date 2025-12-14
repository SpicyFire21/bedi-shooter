using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemInstance item; 
    public Image itemVisual;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;

         // armures
        if (item.data.itemType == ItemType.Equipment && item.data.EquipmentSlot != EquipmentSlot.Hands)
        {
            Equipment equipment = item.data.itemPrefab.GetComponent<Equipment>();

            TooltipSystem.instance.Show(
                item.data.itemName + ", Lv. " + item.data.itemLevel,
                item.data.itemDescription
                + "\n<color=green>Bonus health: " + equipment.equipmentHealthBonus + "</color>"
                + "\n<color=red>Bonus damage: " + equipment.equipmentDamageBonus + "</color>"
                + "\n<color=white>Bonus movespeed: " + equipment.equipmentSpeedBonus + "</color>"
            );

            return;
        }

        // armes
        if (item.data.itemType == ItemType.Equipment && item.data.EquipmentSlot == EquipmentSlot.Hands)
        {
            Weapon weapon = item.data.itemPrefab.GetComponent<Weapon>();

            if (weapon is UltimateMeleeWeapon)
            {
                TooltipSystem.instance.Show(
                     "<color=red>[Ultimate Weapon]</color>\n" + item.data.itemName + ", Lv. " + item.data.itemLevel,
                     item.data.itemDescription
                     + "\n<color=red>Damage: " + weapon.weaponDamage + "</color>"
                     + "\n<color=orange>Weapon attack speed: " + weapon.weaponAttackSpeed + "</color>"
                     + "\n<color=grey>Right click ability:</color> " + "<color=purple>" + (weapon as UltimateMeleeWeapon).rightClickAbilityName + "</color>"
                     + "\n<color=grey>Special ability:</color> " + "<color=purple>" + (weapon as UltimateMeleeWeapon).specialAbilityName + "</color>"
                 );
            } else
            {
                TooltipSystem.instance.Show(
                                 item.data.itemName + ", Lv. " + item.data.itemLevel,
                                 item.data.itemDescription
                                 + "\n<color=red>Damage : " + weapon.weaponDamage + "</color>"
                                 + "\n<color=orange>Weapon attack speed : " + weapon.weaponAttackSpeed + "</color>"
                             );
            }

            return;
        }

        // tout le reste ! 
        TooltipSystem.instance.Show(
            item.data.itemName + ", Lv. " + item.data.itemLevel,
            item.data.itemDescription
        );
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.instance.Hide();
    }

    public void ClickOnSlot()
    {
        Inventory.instance.OpenActionPanel(item, transform.position);
    }
}
