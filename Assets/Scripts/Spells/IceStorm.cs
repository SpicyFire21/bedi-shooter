using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class IceStorm : SpellBase
{
    [Header("Ice Zone Settings")]
    public float lifeTime = 3f;
    public float tickRate = 1f;

    private HashSet<Character> targets = new HashSet<Character>();
    private Coroutine damageCoroutine;
    private bool isActiveZone = true;

    protected override void Cast()
    {
        lifeTime = lifeTime + 0.1f; // sans cela, pour lifetime = 3 et tickrate = 1, les cibles subiront uniquement deux ticks de degats et non 3

        // démarre la coroutine pour les dégâts
        damageCoroutine = StartCoroutine(DamageRoutine());

        // destruction automatique
        Invoke(nameof(DestroyZone), lifeTime);
    }


    // coroutine : méthode qui s'exécute sur plusieurs frames et peut faire des pauses sans bloquer le jeu.
    // ici, elle applique périodiquement les dégâts aux cibles présentes dans la zone.
    private IEnumerator DamageRoutine()
    {
        float damagePerTick = data.damage * (tickRate / lifeTime);
        if (damagePerTick <= 0f) damagePerTick = 1f; // on assure au moins un degat par tick peut importe le calcul

        while (isActiveZone)
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.TakeDamage(damagePerTick);
                }
            }
            yield return new WaitForSeconds(tickRate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveZone) return;

        Character target = other.GetComponent<Character>();
        if (target != null && targets.Add(target))
        {
            Debug.Log("Caracter added:" + target.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActiveZone) return;

        Character target = other.GetComponent<Character>();
        if (target != null && targets.Remove(target))
        {
            Debug.Log("Caracter retired:" + target.name);
        }
    }

    private void DestroyZone()
    {
        isActiveZone = false;
        targets.Clear();
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
    }
}
