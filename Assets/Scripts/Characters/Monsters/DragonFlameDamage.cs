using UnityEngine;
using System.Collections;

public class DragonFlameDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerTick = 9f;
    public float tickRate = 0.3f;

    private Coroutine damageRoutine = null;
    private Collider damageCollider;

    // =====================================================
    // INITIALISATION
    // =====================================================

    private void Start()
    {
        // Récupère le Collider attaché à ce GameObject
        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            // Le Collider est désactivé par défaut (pas de dégâts au repos)
            damageCollider.enabled = false;
        }
    }

    // =====================================================
    // FONCTIONS D'ANIMATION EVENT (Appelées directement par l'Animation)
    // =====================================================

    // ⭐ Fonction appelée par l'Animation Event "StartFireBreath"
    public void StartDealingDamage()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = true; // ⭐ Active la zone de dégâts
            Debug.Log("Flame Damage Collider activé.");
        }
    }

    // ⭐ Fonction appelée par l'Animation Event "StopFireBreath"
    public void StopDealingDamage()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false; // ⭐ Désactive la zone de dégâts
            Debug.Log("Flame Damage Collider désactivé.");
        }

        // Arrêter la coroutine de dégâts immédiatement (DoT)
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    // =====================================================
    // GESTION DES COLLISION ET DES DÉGÂTS
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si le joueur entre dans la zone (Collider activé par l'Animation Event)
        Player player = other.GetComponent<Player>();
        if (player != null && damageRoutine == null)
        {
            // Démarre la coroutine de DoT uniquement si le joueur entre
            damageRoutine = StartCoroutine(DealDamage(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si le joueur sort, on stoppe la coroutine immédiatement
        Player player = other.GetComponent<Player>();
        if (player != null && damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DealDamage(Player player)
    {
        // La coroutine s'exécute TANT QUE le Collider est actif
        while (damageCollider.enabled && player != null)
        {
            player.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickRate);
        }
        // Nettoyage si la boucle se termine (ex: le joueur meurt)
        damageRoutine = null;
    }
}