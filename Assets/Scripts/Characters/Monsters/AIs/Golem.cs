using UnityEngine;

public class Golem : Monster
{
    [Header("Golem Settings")]
    public float stompRange = 3f;       // portée d'attaque au sol
    //public float stompCooldown = 2.5f;  // cooldown d’attaque
    public float stompWindup = 0.7f;    // temps de préparation avant dégâts
    public AudioClip stompSound;

    [Header("Punch Settings")]
    public float attackCooldown = 5f;                // cooldown du punch
    public float punchRange = 2f;                  // portée du poing
    public float punchDamageMultiplier = 3f;       // dégâts multipliés
    public float punchKnockbackStrength = 18f;      // plus fort que stomp
    public AudioClip punchSound;

    private float lastStompTime;
    private float lastPunchTime;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();

        // Le golem est lent mais puissant
        moveSpeed *= 0.6f;
        agent.acceleration *= 0.5f;
        agent.angularSpeed *= 0.6f;
    }

    public override void Update()
    {
        base.Update();
        if (isDead || isAttacking || player == null) return;

        LookAtPlayer();
        HandleMovement();
        TryAttack();
    }

    private void HandleMovement()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stompRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? 1f : 0f);
        }
        else
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0f);
        }
    }

    protected virtual void LookAtPlayer()
    {
        if (player == null || isAttacking) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > data.detectionRange) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;

        float rotationSpeed = distance <= data.attackRange ? 3f : 2f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotationSpeed);
    }

    private void PlayRandomEmoteAfterAttack()
    {
        // Probabilité de faire l'emote : ajustable (ici 25%)
        float emoteChance = 0.25f;

        // On efface toujours les vieux triggers pour éviter un blocage
        anim.ResetTrigger("Emote1");

        // Tirage aléatoire
        if (Random.value <= emoteChance)
        {
            Debug.Log("Golem plays emote!");
            anim.SetTrigger("Emote1");
        }
    }

    public void EndAttack()
    {
        //Debug.Log("EndAttack() called");

        isAttacking = false;
        agent.isStopped = false;
        lastAttackTime = Time.time;  // pour le cooldown

        PlayRandomEmoteAfterAttack();

        // Optionnel : relancer la poursuite
        if (player != null)
            agent.SetDestination(player.position);
    }

    public void DebugEvent()
    {
        Debug.Log("DEBUG EVENT CALLED");
    }

    private float storedDamage;
    public override void Attack(float damage)
    {
        //Debug.Log("Attack appeler");
        storedDamage = damage;

        // Choisir une attaque aleatoire : 0 = Stomp, 1 = Punch
        int randomAttack = Random.Range(0, 2);
        anim.SetInteger("AttackIndex", randomAttack);
        anim.SetTrigger("Attack");

        lastAttackTime = Time.time;
    }

    private void TryAttack()
    {
        //Debug.Log("TryAttack() called");

        if (player == null)
        {
            Debug.Log("Player == null");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        //Debug.Log("Distance: " + distance);

        if (distance > stompRange)
        {
            //Debug.Log("Too far to attack");
            return;
        }

        if (isAttacking)
        {
            Debug.Log("Blocked: isAttacking == true");
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
        {
            //Debug.Log("Cooldown not finished");
            return;
        }

        Debug.Log("🟢 ATTACK LAUNCHED !");
        isAttacking = true;
        agent.isStopped = true;

        Attack(damage);
    }



    // Appel via Animation Event pour Stomp
    private void DealStompDamage()
    {
        //Debug.Log("Golem are stomping");
        Collider[] hits = Physics.OverlapSphere(transform.position, stompRange);
        foreach (Collider hit in hits)
        {
            Player playerCharacter = hit.GetComponent<Player>();
            if (playerCharacter != null)
                playerCharacter.TakeDamage(damage * 2f);

            PlayerKnockback knock = hit.GetComponent<PlayerKnockback>();
            if (knock != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                knock.ApplyKnockback(dir, 10f, 0.15f, 0.4f);
            }
        }
        isAttacking = false;
        agent.isStopped = false;
    }

    private void DealPunchDamage()
    {
        // Centre du punch (au niveau du Golem)
        Vector3 punchCenter = transform.position;
        // Rayon du punch
        float radius = punchRange;
        // Détecte tous les colliders dans la zone du punch
        Collider[] hits = Physics.OverlapSphere(punchCenter, radius);

        if (hits.Length == 0)
        {
            Debug.Log("Punch: no valid target hit.");
        }

        foreach (Collider hit in hits)
        {
            // Cherche le composant Player sur le GameObject ou ses enfants
            Player playerCharacter = hit.GetComponent<Player>();
            if (playerCharacter != null)
                playerCharacter.TakeDamage(damage * punchDamageMultiplier);

            PlayerKnockback knock = hit.GetComponent<PlayerKnockback>();
            if (knock != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                knock.ApplyKnockback(dir, punchKnockbackStrength, 0.01f, 0.5f);
            }

            //Debug.Log("Punch hits the player!");
        }
        // Fin de l'attaque
        isAttacking = false;
        agent.isStopped = false;
    }

    public void FootStomp()
    {
        // Intensité et durée ajustables
        float shakeIntensity = 2f;
        float shakeDuration = 0.3f;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeIntensity, shakeDuration);
    }

    public void Footstep()
    {
        // Empêche les tremblements si golem mort / attaquant
        if (isDead) return;

        // Intensité et durée ajustables
        float shakeIntensity = 0.2f;  // tu peux ajuster
        float shakeDuration = 0.2f;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeIntensity, shakeDuration);
    }


    public void SlowDeathAnimation()
    {
        if (anim != null)
        {
            anim.speed = 0.2f; // ralentit l'animation
        }
    }

    public void SpeedDeathAnimation()
    {
        if (anim != null)
        {
            anim.speed = 1f;
        }
    }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}
