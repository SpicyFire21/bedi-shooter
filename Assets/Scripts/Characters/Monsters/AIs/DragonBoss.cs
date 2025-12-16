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

    [Header("VFX References")]
    public ParticleSystem fireBreathVFX;

    [Header("Sounds Effects")]
    public AudioClip screamSound;
    public AudioClip FireSound;
    public AudioClip flyingSound;
    public AudioClip crocSound;
    public AudioClip jumpSound;
    public AudioClip chargeScreamSound;

    [Header("Movement")]
    public float groundSpeed = 5f;
    public float airSpeed = 10f; // Vitesse utilisée pour la rotation en vol
    public float groundAttackRotationSpeed = 8f; // Vitesse de rotation au sol pour faire face au joueur

    [Header("Attack Damage Multipliers")]
    public float biteDamageMultiplier = 1.0f;
    public float chargeDamageMultiplier = 1.5f;
    public float flameDamageMultiplier = 0.5f; // Dégâts plus faibles mais peut être DoT
    public float flameDamagePerTick = 0.1f;    // Dégâts pour l'attaque aérienne (si DoT)

    [Header("Fire Breath Damage")]
    [SerializeField] private DragonFlameDamage flameDamage;

    [Header("Knockback Settings")]
    public float biteKnockback = 8f;
    public float chargeKnockback = 20f; // Très fort pour une charge
                                        // Durée et hauteur des Knockbacks peuvent être ajustées dans les méthodes
    [Header("Attack Hitbox Offset")]
    public float biteOffsetDistance = 5.6f; // Décalage du centre de la morsure devant le Dragon
    public float chargeOffsetDistance = 8.5f; // Décalage du centre de la charge devant le Dragon
    public float biteRadius = 2f;
    public float chargeRadius = 3f;

    [Header("Fighting Distance")]
    public float biteRange = 8f;
    public float flameRangeGround = 20f;
    public float chargeRange = 15f;
    public float stoppingDistanceBuffer = 4.0f;

    [Header("Cooldowns")]
    public float attackCooldown = 2f;
    public float chargeCooldown = 8f;
    public float flameCooldown = 10f;
    public float screamCooldown = 15f;

    [Header("Phase Settings")]
    public float phase2HealthRatio = 0.66f;
    public float phase3HealthRatio = 0.33f;

    private float lastAttackTime;
    private float lastFlameTime;
    private float lastChargeTime;
    private float lastScreamTime;

    private Rigidbody rb;

    private bool shouldWarpAfterAttack = false;

    private bool isAttacking = false;
    private bool phase2Triggered = false;
    private bool phase3Triggered = false;
    private bool isInFlyingEvent = false;

    private void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.updateRotation = false;

        if (fireBreathVFX != null)
        {
            // crache pas de feu
            fireBreathVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void Update()
    {
        base.Update();
        if (isDead || player == null || isInFlyingEvent)
            return;

        CheckPhaseTransitions();

        anim.SetFloat("Speed", agent.velocity.magnitude);

        // Comportement au sol ou rotation d'attaque
        if (!isInFlyingEvent && !isAttacking)
        {
            GroundBehaviour();
        }
        else if (isAttacking && !isInFlyingEvent)
        {
            LookAtPlayer(groundAttackRotationSpeed * 1.5f);
        }
    }

    // Gère la rotation rapide vers le joueur lors des attaques au sol
    private void LookAtPlayer(float rotationSpeed)
    {
        if (player == null || isAttacking)
            return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * rotationSpeed
        );
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

        if (!agent.enabled || player == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // -----------------------------
        // Rotation vers le joueur
        // -----------------------------
        LookAtPlayer(groundAttackRotationSpeed);

        // -----------------------------
        // Mouvement via NavMeshAgent UNIQUEMENT
        // -----------------------------
        if (distance > biteRange) // distance d'arrêt logique
        {
            agent.isStopped = false;
            agent.speed = groundSpeed;
            agent.SetDestination(player.position);

            anim.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? 1f : 0f);
        }
        else
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0f);
        }

        // -----------------------------
        // Attaques
        // -----------------------------
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

        if (distance < flameRangeGround && time > lastFlameTime + flameCooldown && distance > biteRange)
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
            if (trigger == "Charge")
            {
                agent.enabled = false;
                rb.useGravity = false;
                rb.isKinematic = true;

                // 🔒 LOCK rotation
                LookAtPlayer(999f);

                shouldWarpAfterAttack = true;
            }
            else
            {
                // Pour les autres attaques, on le stoppe simplement
                agent.isStopped = true;
            }
            LookAtPlayer(groundAttackRotationSpeed * 5f);
        }

        // Réinitialisation explicite des autres triggers
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Charge");
        anim.ResetTrigger("Flamme_Attack");
        anim.ResetTrigger("Scream");

        anim.SetTrigger(trigger);
    }

    // =================ATTACK EVENT========================
    public void DealBiteDamage()
    {
        // Calcule la zone de dégâts DEVANT le Dragon en utilisant l'offset
        Vector3 biteCenter = transform.position + transform.forward * biteOffsetDistance;

        // Détecte les cibles dans la zone
        Collider[] hits = Physics.OverlapSphere(biteCenter, biteRadius);

        foreach (Collider hit in hits)
        {
            Player playerCharacter = hit.GetComponent<Player>();
            if (playerCharacter != null)
            {
                // ... (logique de dégâts et knockback)
                playerCharacter.TakeDamage(damage * biteDamageMultiplier);

                PlayerKnockback knock = hit.GetComponent<PlayerKnockback>();
                if (knock != null)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    knock.ApplyKnockback(dir, biteKnockback, 0.1f, 0.3f);
                }
                break;
            }
        }
    }

    public void DealChargeDamage()
    {
        // Calcule la zone de dégâts DEVANT le Dragon en utilisant l'offset
        Vector3 chargeCenter = transform.position + transform.forward * chargeOffsetDistance;

        // Détecte les cibles dans la zone
        Collider[] hits = Physics.OverlapSphere(chargeCenter, chargeRadius);

        foreach (Collider hit in hits)
        {
            Player playerCharacter = hit.GetComponent<Player>();
            if (playerCharacter != null)
            {
                // ... (logique de dégâts et knockback)
                playerCharacter.TakeDamage(damage * chargeDamageMultiplier);

                PlayerKnockback knock = hit.GetComponent<PlayerKnockback>();
                if (knock != null)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    knock.ApplyKnockback(dir, chargeKnockback, 0.2f, 0.6f);
                }
                break;
            }
        }
    }

    // =====================================================
    //                   FLYING EVENT
    // =====================================================
    private IEnumerator FlyingPhaseRoutine(int phaseIndex)
    {
        isInFlyingEvent = true;
        isAttacking = true;

        // Assurez-vous que l'Agent est arrêté immédiatement
        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Le Rigidbody DOIT être Kinematic pour que le LERP dans FlyTo fonctionne
        rb.useGravity = false;
        rb.isKinematic = true;

        anim.SetBool("Is_Flying", true);

        if (flyingSound != null && player != null)
            AudioSource.PlayClipAtPoint(flyingSound, player.position, 1f);

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

        if (flyingSound != null && player != null)
            AudioSource.PlayClipAtPoint(flyingSound, player.position, 1f);

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

        // CAS DE LA CHARGE
        if (shouldWarpAfterAttack)
        {
            shouldWarpAfterAttack = false;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.enabled = true;
                agent.Warp(hit.position);
                rb.isKinematic = false;     // ⭐ réactive la physique
                rb.useGravity = true;       // ⭐ réactive la gravité
                agent.isStopped = false;

                // ⭐ FIX CRITIQUE : forcer une destination
                agent.SetDestination(player.position);
            }
            else
            {
                Debug.LogWarning("❌ Échec critique du repositionnement après Charge.");

                agent.enabled = true;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
            }
        }
        else
        {
            // Autres attaques (morsure, flamme, cri)
            if (agent.enabled && agent.isOnNavMesh && !isInFlyingEvent)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position); // ⭐ sécurité
            }
        }
    }

    public void ScreamCameraShake()
    {
        // Empêche l'exécution si le Dragon est mort ou si l'instance de CameraShake n'existe pas
        if (isDead)
            return;

        if (CameraShake.Instance != null)
        {
            Debug.Log("🔊 Dragon hurle : tremblement de caméra déclenché.");
            // Appelle la fonction de tremblement en utilisant les variables de l'inspecteur
            CameraShake.Instance.Shake(10f, 2f);
        }
    }

    public void DeathCameraShake()
    {
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(5f, 1f);
        }
    }

    public void StartFireBreath()
    {
        if (anim != null)
        {
            anim.speed = 0.6f; // ralentit l'animation
        }
        if (fireBreathVFX != null)
        {
            // ⭐ Démarre l'émission des particules
            fireBreathVFX.Play();
            Debug.Log("🔥 Fire Breath DÉMARRE.");

            // Optionnel : Ajoutez ici le début de votre logique de dégâts de DoT (Damage over Time)
        }
    }
    public void StopFireBreath()
    {
        if (anim != null)
        {
            anim.speed = 1f;
        }
        if (fireBreathVFX != null)
        {
            // ⭐ Arrête l'émission, mais permet aux particules existantes de s'éteindre
            fireBreathVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Debug.Log("🔥 Fire Breath S'ARRÊTE.");

            // Optionnel : Ajoutez ici la fin de votre logique de dégâts de DoT
        }
    }

    public void StartFireBreathDamage()
    {
        if (flameDamage != null)
        {
            flameDamage.StartDealingDamage();
        }
    }

    // Appelé par Animation Event
    public void StopFireBreathDamage()
    {
        if (flameDamage != null)
        {
            flameDamage.StopDealingDamage();
        }
    }

    //==================================================
    //                 Sound Effect
    //==================================================
    public void CrocSoundPlay()
    {
        if (crocSound != null)
            AudioSource.PlayClipAtPoint(crocSound, transform.position, 1f);
    }

    public void FireSoundPlay()
    {
        if (FireSound != null && player != null)
            AudioSource.PlayClipAtPoint(FireSound, player.position, 4f);
    }

    public void ScreamSoundPlay()
    {
        if (screamSound != null && player != null)
            AudioSource.PlayClipAtPoint(screamSound, player.position, 1f);
    }

    public void ChargeSoundPlay()
    {
        if (jumpSound != null && player != null)
            AudioSource.PlayClipAtPoint(jumpSound, transform.position, 1f);
    }
    public void ChargeScreamSoundPlay()
    {
        if (chargeScreamSound != null && player != null)
            AudioSource.PlayClipAtPoint(chargeScreamSound, transform.position, 1f);
    }

    // =====================================================
    //                 OVERRIDES (MONSTER)
    // =====================================================

    /*public void DeathHandler()
    {
        //base.DeathHandler();
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

        anim.SetTrigger("Is_Dead");

        // Logique de fin de combat (loot, cinématique...)
    }*/

    public override void Attack(float damage) { }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}