using UnityEngine;

public class Skeleton : Monster
{

    void start()
    {
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
            LaunchAttack();
        }
    }

    private void LaunchAttack()
    {
        int randomAttack = Random.Range(0, 5); // 0 → 4

        anim.SetInteger("AttackIndex", randomAttack);
        anim.SetTrigger("Attack");

        lastAttackTime = Time.time;
    }
}
