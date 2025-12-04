using UnityEngine;

public class Skeleton : Monster
{

    public PirateSword weapon;
    private float storedDamage;

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

    private void LookAtPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Ne regarde le joueur que lorsqu'il est suffisant proche pour attaquer
        if (distance > data.attackRange + 0.2f)
            return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (distance <= data.detectionRange && distance > data.attackRange)
        {
            // rotation légère pendant la marche
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 2f
            );
        }

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 5f
            );
        }
    }


    private void SpeedAnimManager()
    {
        if (agent == null || isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Déplacement
        agent.SetDestination(player.position);

        bool inAttackRange = distance <= data.attackRange;

        // Stop uniquement pendant le LANCEMENT de l'attaque
        agent.isStopped = inAttackRange;

        // Vitesse : course ou marche
        agent.speed = (distance > data.detectionRange) ? moveSpeed * 3f : moveSpeed;

        // Animation de déplacement
        float animSpeed = (distance > data.detectionRange) ? 2f : 1f;
        anim.SetFloat("Speed", agent.velocity.magnitude > 0.1f ? animSpeed : 0f);

        // --- LOGIQUE D’ATTAQUE ---
        if (inAttackRange && Time.time >= lastAttackTime + data.attackCooldown)
        {
            if (weapon != null)
            {
                if (weapon.weaponDamage < (damage / 2)) // si il a une arme vraiment faible par rapport a son niveau, on va faire un petit calcul 
                    // entre ses degats et les degats de son arme pour pas qu'il tape du 7 si par exemple les degats de son arme c'est 7
                {
                    Attack(weapon.weaponDamage + damage / 1.5f);
                } else
                {
                    Attack(weapon.weaponDamage);
                }
            } else
            {
                Attack(damage);
            }
        }
    }

    public override void Attack(float damage)
    {
        Debug.Log("ATTACK");
        storedDamage = damage;

        int randomAttack = Random.Range(0, 5); // 0 → 4
        anim.SetInteger("AttackIndex", randomAttack);
        anim.SetTrigger("Attack");
        AudioSource.PlayClipAtPoint(weapon.actionSound, transform.position, 1f);
        lastAttackTime = Time.time;
    }

    public void ApplyDamage()
    {
        if (player != null)
        {
            Player playerCharacter = player.GetComponent<Player>();
            if (playerCharacter != null)
            {
                playerCharacter.TakeDamage(storedDamage);
            }
        }
    }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        Debug.Log("un monstre a spawn : " + data.name);
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }

}
