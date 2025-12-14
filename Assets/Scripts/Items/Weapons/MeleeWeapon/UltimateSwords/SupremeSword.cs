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
    private float preAbilityDuration = 1.2f;
    public float finalAbilityDuration;
    private Vector3 startCastPos;

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
        startCastPos = GetRightClickPosition(player, 6f);
        startCastPos.y = player.castPoint.position.y + 1f;
        GameObject precastObject = Instantiate(visualPreAbility, startCastPos, Quaternion.identity);
        if (preAbilityAudio != null)
        {
            AudioSource.PlayClipAtPoint(
                preAbilityAudio,
                player.transform.position,
                2f
            );
        }
        player.tps.enabled = false;
        ActiveUnactiveAnimation(player, precastObject, preAbilityDuration);
    }

    private void ActiveUnactiveAnimation(Player player, GameObject obj, float duration)
    {
        player.StartCoroutine(ScalePulseCoroutine(player, obj, duration));
    }

    private IEnumerator ScalePulseCoroutine(Player player, GameObject obj, float duration)
    {
        float elapsed = 0f;
        float value = 0.3f;

        while (elapsed < duration)
        {
            float random = Random.Range(0f, 1f);

            obj.SetActive(random <= value);

            value += 0.013f; // augmente progressivement la probabilité
            elapsed += Time.deltaTime;
            yield return null; // attendre une frame
        }

        obj.SetActive(true); // état final
        FinalAbilityCast(player, finalAbilityDuration);
    }

    public void FinalAbilityCast(Player player, float duration) 
    {
        AudioSource.PlayClipAtPoint(
                specialAbilitySound,
                player.transform.position,
                2f
            );

        Instantiate(visualFinalAbility, startCastPos, Quaternion.identity);

        player.tps.enabled = true;
    }







}
