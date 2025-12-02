using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HolySword : ZoneSpellBase
{
    [Header("Holy Sword Settings")]
    public float tickRate = 1f;
    private Coroutine healthRoutine;


    public override void Initialize(Character caster, SpellData data)
    {
        base.Initialize(caster, data);
        healthRoutine = StartCoroutine(HealthRoutine()); // initialisation de base + notre customization par rapport au spell (ici des damages mais on peut bien avoir du heal...)
    }


    // coroutine : méthode qui s'exécute sur plusieurs frames et peut faire des pauses sans bloquer le jeu.
    // ici, elle applique périodiquement les dégâts aux cibles présentes dans la zone.
    private IEnumerator HealthRoutine()
    {
        float damagePerTick = data.value * (tickRate / data.lifeTime);
        if (damagePerTick <= 0f) damagePerTick = 1f; // on assure au moins un degat par tick peut importe le calcul
        while (isActiveZone)
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.TakeHealth(damagePerTick);
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

    private void OnDestroy()
    {
        if (healthRoutine != null)
            StopCoroutine(healthRoutine);

        isActiveZone = false;
    }
}
