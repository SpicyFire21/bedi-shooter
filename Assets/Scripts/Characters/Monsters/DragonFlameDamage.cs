using UnityEngine;
using System.Collections;

public class DragonFlameDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerTick = 5f;
    public float tickRate = 0.5f;

    private Coroutine damageRoutine;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            damageRoutine = StartCoroutine(DealDamage(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    private IEnumerator DealDamage(Player player)
    {
        while (player != null)
        {
            player.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickRate);
        }
    }
}