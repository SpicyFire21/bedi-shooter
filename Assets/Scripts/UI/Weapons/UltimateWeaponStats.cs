using UnityEngine;
using UnityEngine.UI;

public class UltimateWeaponStats : MonoBehaviour
{
    [SerializeField] private Text weaponName;
    [SerializeField] private Text rightClickText;
    [SerializeField] private Text abilityText;
    [SerializeField] private Player player;

    void Update()
    {
        UltimateMeleeWeapon equipped = player.GetUltimateMeleeWeapon();

        if (equipped != null)
        {
            weaponName.text =
                "<color=red>" + equipped.data.itemName.ToString() +
                "</color>, Lv. " + equipped.data.itemLevel;

            float rightCd = equipped.GetCurrentRightClickCooldown();

            if (rightCd > 0f)
            {
            rightClickText.text =
                    "Right click to [" + equipped.rightClickAbilityName + "] " +
                    "<color=red>" + rightCd.ToString("F1") + "s</color>";
            }
            else
            {
            rightClickText.text =
                    "Right click to [" + equipped.rightClickAbilityName + "] " +
                    "<color=green>Ready</color>";
            }

            abilityText.text = "Press V to use [" + equipped.specialAbilityName + "]";
        }
        else
        {
            weaponName.text = "";
            rightClickText.text = "";
            abilityText.text = "";
        }
    }

}
