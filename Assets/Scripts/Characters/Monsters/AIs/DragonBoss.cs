using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DragonBoss : Monster
{
    [Header("References")]
    public Transform player;

    [Header("Flight Points")]
    public Transform outOfArenaPoint;
    public Transform fireBreathPoint;
    public Transform landingPoint;

    [Header("Movement")]
    public float groundSpeed = 5f;
    public float airSpeed = 10f; // Vitesse utilisée pour la rotation en vol
    public float groundAttackRotationSpeed = 8f; // Vitesse de rotation au sol pour faire face au joueur

    [Header("Fighting Distance")]
    public float biteRange = 3f;
    public float flameRangeGround = 10f;
    public float chargeRange = 12f;

    [Header("Cooldowns")]
    public float attackCooldown = 2f;
    public float chargeCooldown = 6f;
    public float flameCooldown = 5f;
    public float screamCooldown = 12f;

    [Header("Phase Settings")]
    public float phase2HealthRatio = 0.66f;
    public float phase3HealthRatio = 0.33f;

    private float lastAttackTime;
    private float lastFlameTime;
    private float lastChargeTime;
    private float lastScreamTime;

    private Rigidbody rb;

    private bool isAttacking = false;
    private bool phase2Triggered = false;
    private bool phase3Triggered = false;
    private bool isInFlyingEvent = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.updateRotation = true;
    }

    private void Update()
    {
        if (isDead || player == null || isInFlyingEvent)
            return;

        CheckPhaseTransitions();

        anim.SetFloat("Speed", agent.velocity.magnitude);

        // Comportement au sol ou rotation d'attaque
        if (!isInFlyingEvent && !isAttacking)
        {
            GroundBehaviour();
        }
        else if (isAttacking && agent.enabled)
        {
            // Permet une rotation rapide pendant les animations d'attaque au sol
            RotateTowardsPlayer(groundAttackRotationSpeed);
        }
    }

    // Gère la rotation rapide vers le joueur lors des attaques au sol
    private void RotateTowardsPlayer(float rotationSpeed)
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // =====================================================
    //                     PHASE CHECK
    // =====================================================
    private void CheckPhaseTransitions()
    {
        if (isInFlyingEvent || maxHealth <= 0) return;

        float healthRatio = currentHealth / maxHealth;

        if (!phase2Triggered && healthRatio <= phase2HealthRatio)
        {
            phase2Triggered = true;
            StartCoroutine(FlyingPhaseRoutine(2));
        }
        else if (!phase3Triggered && healthRatio <= phase3HealthRatio)
        {
            phase3Triggered = true;
            StartCoroutine(FlyingPhaseRoutine(3));
        }
    }

    // =====================================================
    //                   GROUND COMBAT
    // =====================================================
    private void GroundBehaviour()
    {
        anim.SetBool("Is_Flying", false);
        agent.speed = groundSpeed;

        if (agent.enabled && agent.isOnNavMesh)
            agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        TryGroundAttacks(distance);
    }

    private void TryGroundAttacks(float distance)
    {
        float time = Time.time;

        if (distance < biteRange && time > lastAttackTime + attackCooldown)
        {
            TriggerAttack("Attack");
            lastAttackTime = time;
            return;
        }

        if (distance < chargeRange && time > lastChargeTime + chargeCooldown)
        {
            TriggerAttack("Charge");
            lastChargeTime = time;
            return;
        }

        if (distance < flameRangeGround && time > lastFlameTime + flameCooldown)
        {
            TriggerAttack("Flamme_Attack");
            lastFlameTime = time;
            return;
        }

        if (time > lastScreamTime + screamCooldown)
        {
            TriggerAttack("Scream");
            lastScreamTime = time;
        }
    }

    private void TriggerAttack(string trigger)
    {
        isAttacking = true;

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            // Force la rotation vers le joueur immédiatement
            RotateTowardsPlayer(groundAttackRotationSpeed * 5f);
        }

        // ⭐ AJOUT : Réinitialisation explicite des autres triggers
        // C'est une mesure de sécurité pour s'assurer qu'un trigger précédent ne se réactive pas
        // ou n'interfère pas avec la nouvelle attaque.
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Charge");
        anim.ResetTrigger("Flamme_Attack");
        anim.ResetTrigger("Scream");

        // On définit le nouveau Trigger d'animation
        anim.SetTrigger(trigger);
    }

    // =====================================================
    //                   FLYING EVENT
    // =====================================================
    private IEnumerator FlyingPhaseRoutine(int phaseIndex)
    {
        isInFlyingEvent = true;
        isAttacking = true;

        agent.isStopped = true;
        agent.enabled = false;

        rb.useGravity = false;
        // ⭐ AJOUT : Rend le Rigidbody insensible aux forces externes
        rb.isKinematic = true;

        anim.SetBool("Is_Flying", true);

        // 🛫 Takeoff
        yield return new WaitForSeconds(2.5f);

        // 🗺 Quit arena (Durée augmentée à 5s)
        yield return FlyTo(outOfArenaPoint.position, 5f);

        // 👹 Spawn adds
        SpawnAdds(phaseIndex);

        yield return new WaitForSeconds(4f);

        // 🔥 Fire breath over arena (Durée augmentée à 6s)
        yield return FlyTo(fireBreathPoint.position, 6f);
        anim.SetTrigger("Flamme_Attack");

        yield return new WaitForSeconds(3f);

        // 🛬 Landing (Durée augmentée à 4s)
        yield return FlyTo(landingPoint.position, 4f);

        LandDragon();

        isInFlyingEvent = false;
        isAttacking = false;
    }

    private IEnumerator FlyTo(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Stocker la position avant la mise à jour pour calculer la direction
            Vector3 previousPosition = transform.position;

            // Déplacement LERP
            transform.position = Vector3.Lerp(start, target, t);

            // CHANGEMENT : Calcul de la direction du mouvement
            Vector3 flightDir = (transform.position - previousPosition).normalized;

            if (flightDir.sqrMagnitude > 0.01f)
            {
                // Rotation vers la direction du mouvement
                Quaternion lookRotation = Quaternion.LookRotation(flightDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    lookRotation,
                    Time.deltaTime * airSpeed
                );
            }

            yield return null;
        }

        // S'assurer d'être exactement à la cible
        transform.position = target;
    }

    private void LandDragon()
    {
        rb.useGravity = true;
        // ⭐ AJOUT : Permet au Rigidbody d'être à nouveau dynamique
        rb.isKinematic = false;

        anim.SetBool("Is_Flying", false);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 15f, NavMesh.AllAreas))
        {
            transform.position = hit.position;

            agent.enabled = true;
            agent.Warp(hit.position);
            agent.isStopped = false;
        }
        else
        {
            Debug.LogWarning("❌ Dragon failed to land on NavMesh. Placing it at origin as fallback.");
            agent.enabled = true;
            agent.Warp(Vector3.zero);
            agent.isStopped = false;
        }
    }

    // =====================================================
    //                   ADD SPAWNING
    // =====================================================
    private void SpawnAdds(int phase)
    {
        Debug.Log($"🐲 Dragon spawns adds for phase {phase}");
        // Votre logique de spawner d'ennemis ici
    }

    // =====================================================
    //               ANIMATION EVENT
    // =====================================================
    public void EndAttack()
    {
        isAttacking = false;

        if (agent.enabled && agent.isOnNavMesh && !isInFlyingEvent)
            agent.isStopped = false;
    }

    // =====================================================
    //                 OVERRIDES (MONSTER)
    // =====================================================

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Dragon Boss defeated!");

        StopAllCoroutines();

        if (agent != null && agent.enabled)
            agent.enabled = false;

        // Assurez-vous que l'objet n'est plus physique
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        anim.SetTrigger("Die");

        // Logique de fin de combat (loot, cinématique...)
    }

    public override void Attack(float damage) { }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}