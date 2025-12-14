using UnityEngine;
using System.Collections;

public class SupremeSword : UltimateMeleeWeapon
{
    [Header("Prefabs")]
    public GameObject visualImpact;
    public GameObject visualPreAbility;
    public GameObject visualFinalAbility;

    [Header("Audios")]
    public AudioClip impactAudio;
    public AudioClip preAbilityAudio;
    public AudioClip finalAbilityAudio;

    [Header("Durations")]
    public float visualImpactDuration;
    public float finalAbilityDuration;
    private Vector3 startCastPos;

    [Header("Open stats")]
    public float arrowSpeed = 10f;
    private Rigidbody rb;

    public override void DoAnimation(Player player)
    {
        player.animator.SetFloat("AttackSpeed", weaponAttackSpeed);
        player.animator.SetTrigger("SupremeSwordAnimation");
        player.tps.canMove = false;
    }

    public override void DoRightClick(Player player)
    {
        if (!CanRightClick())
            return;
        Teleport(player, maxRightClickDistance);
        TriggerRightClickCooldown();
    }

    public override void SpecialAbility(Player player)
    {
        if (!CanCastAbility()) return;

        PrecastAbility(player);
    }

    public void Teleport(Player player, float maxDistance)
    {
        Vector3 targetPosition = GetRightClickPosition(player, maxDistance);
        player.tps.enabled = false;
        if (targetPosition.y < 0) targetPosition.y = 0.1f;
        player.transform.position = targetPosition;
        player.tps.enabled = true;
        AudioSource.PlayClipAtPoint(rightClickAbilitySound, player.transform.position, 1f);
    }

    public override void ImpactEffect(Player player, Monster monster)
    {
        player.StartCoroutine(ImpactCoroutine(monster));
    }

    private IEnumerator ImpactCoroutine(Monster monster)
    {
        GameObject impact = Instantiate(
            visualImpact,
            monster.transform.position,
            Quaternion.identity
        );

        if (impactAudio != null)
        {
            AudioSource.PlayClipAtPoint(
                impactAudio,
                monster.transform.position,
                2f
            );
        }
        yield return new WaitForSeconds(visualImpactDuration); // on attend la durée tranquillement 
        Destroy(impact);
    }

    private void PrecastAbility(Player player)
    {
        TriggerAbilityCooldown();

        Vector3 targetPos = GetRightClickPosition(player, 100f); 

        Vector3 origin = player.castPoint.position;
        Vector3 direction = targetPos - origin;
        float distance = direction.magnitude;

        if (distance > 6f)
        {
            direction = direction.normalized * 6f;
        }

        Vector3 finalTargetPos = origin + direction;
        GameObject precastObject = Instantiate(visualPreAbility, origin, Quaternion.identity);

        if (preAbilityAudio != null)
            AudioSource.PlayClipAtPoint(preAbilityAudio, player.transform.position, 1f);

        rb = precastObject.GetComponent<Rigidbody>();

        Vector3 finalDirection = (finalTargetPos - origin).normalized;
        rb.linearVelocity = finalDirection * arrowSpeed;
        precastObject.transform.forward = finalDirection;


    }


    public void FinalAbilityCast(GameObject precastObject)
    {
        AudioSource.PlayClipAtPoint(
                specialAbilitySound,
                precastObject.transform.position,
                2f
            );
        GameObject open = Instantiate(visualFinalAbility, precastObject.transform.position, Quaternion.identity);
        Destroy(precastObject);
        CameraShake.Instance.Shake(0.5f, 1f);
        float radius = 4f; // rayon de la zone
        Collider[] hitColliders = Physics.OverlapSphere(open.transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            Monster monster = hit.GetComponent<Monster>();

            if (monster != null)
            {
                monster.TakeDamage(specialAbilityDamage);
            }
        }
        Destroy(open, finalAbilityDuration);

    }







}