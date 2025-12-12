using UnityEngine;
using UnityEngine.UI;

public class GunStats : MonoBehaviour
{
    [SerializeField] private Text ammoInfoText;
    [SerializeField] private Text ammoAmountText;
    [SerializeField] private Image bulletImage;
    [SerializeField] private Text reloadReminderText;
    [SerializeField] private Player player;

    private RangeWeapon currentWeapon;

    void Update()
    {
        // Vérifie si le joueur a une arme équipée
        Weapon equipped = player.GetEquippedWeapon();
        if (equipped == null || !(equipped is RangeWeapon))
        {
            ammoInfoText.gameObject.SetActive(false); // cache le texte si aucune arme
            ammoAmountText.gameObject.SetActive(false);
            bulletImage.gameObject.SetActive(false);
            reloadReminderText.gameObject.SetActive(false);
            return;
        }

        currentWeapon = equipped as RangeWeapon;
        ammoInfoText.gameObject.SetActive(true);
        ammoAmountText.gameObject.SetActive(true);
        bulletImage.gameObject.SetActive(true);
        ammoInfoText.text = currentWeapon.getAmmoInMagazine().ToString() + " / " + currentWeapon.magazineSize.ToString();
        ammoAmountText.text = currentWeapon.currentAmmo.ToString();

        if (currentWeapon.getAmmoInMagazine() < ((int) currentWeapon.magazineSize / 5) && currentWeapon.currentAmmo > currentWeapon.getAmmoInMagazine())
        {
            reloadReminderText.gameObject.SetActive(true);
        } else
        {
            reloadReminderText.gameObject.SetActive(false);
        }

    }
}
