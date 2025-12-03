using UnityEngine;

public class PirateSword : MonoBehaviour
{
    public WeaponData weaponData;
    public bool canDealDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(weaponData.damage);
                Debug.Log("Le joueur a pris " + weaponData.damage + " dégâts !");
            }
        }
    }

    public void EnableDamage()
    {
        canDealDamage = true;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
    }
}
