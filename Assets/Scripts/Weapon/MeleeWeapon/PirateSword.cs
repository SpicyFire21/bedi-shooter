using UnityEngine;

public class PirateSword : MeleeWeapon
{

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (!canDealDamage) return;

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(weaponData.weaponDamage);
                Debug.Log("Le joueur a pris " + weaponData.weaponDamage + " dégâts !");
            }
        }
        */
    }
    
}
