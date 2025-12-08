using UnityEngine;

public class Skeleton : Monster
{
    public PirateSword weapon;
    private float storedDamage;

    [Header("Movement Settings")]
    protected float runSpeedMultiplier = 2f;   // Vitesse quand il court (hors detection range)
    protected float runAnimMultiplier = 2f;    // Valeur du paramètre "Speed" en course (chatGPT est vraiment con)

    protected override void Start()
    {
        base.Start();
        agent.updateRotation = true;
    }

    public override void Update()
    {
        base.Update();
        LookAtPlayer();
        SpeedAnimManager();
    }

    protected virtual void LookAtPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Ne regarde le joueur que lorsqu'il est assez proche
        if (distance > data.attackRange + 0.2f) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        // Rotation douce si proche mais pas encore à portée d’attaque
        if (distance <= data.detectionRange && distance > data.attackRange)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 2f
            );
        }

        // Rotation rapide si en attaque
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 5f
            );
        }
    }

    protected virtual void SpeedAnimManager()
    {
        if (agent == null || isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inAttackRange = distance <= data.attackRange;

        // Déplacement
        agent.SetDestination(player.position);
        agent.isStopped = inAttackRange;   // stop pendant attaque

        // --- VITESSE AGENT ---
        if (distance > data.detectionRange)
        {
            // Courir
            agent.speed = moveSpeed * runSpeedMultiplier;
            anim.SetFloat("Speed", runAnimMultiplier);
        }
        else if (!inAttackRange)
        {
            // Marcher
            agent.speed = moveSpeed;
            anim.SetFloat("Speed", 1f);
        }
        else
        {
            // Idle / attaque
            anim.SetFloat("Speed", 0f);
        }

        // --- ATTAQUE ---
        if (inAttackRange && Time.time >= lastAttackTime + data.attackCooldown)
        {
            float dmg = weapon != null ? weapon.weaponDamage : damage;

            if (weapon != null && weapon.weaponDamage < (damage / 2))
                dmg = weapon.weaponDamage + damage / 1.5f;

            Attack(dmg);
        }
    }

    public override void Attack(float damage)
    {
        storedDamage = damage;

        int randomAttack = Random.Range(0, 5);
        anim.SetInteger("AttackIndex", randomAttack);
        anim.SetTrigger("Attack");
      

        lastAttackTime = Time.time;
    }

    public void PlaySwordSound()
    {
        if (weapon != null)
            AudioSource.PlayClipAtPoint(weapon.actionSound, transform.position);
    }

    public void ApplyDamage()
    {
        if (player == null) return;

        Player playerCharacter = player.GetComponent<Player>();
        if (playerCharacter != null)
            playerCharacter.TakeDamage(storedDamage);
    }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}
