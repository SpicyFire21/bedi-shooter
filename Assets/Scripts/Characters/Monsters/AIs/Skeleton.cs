using UnityEngine;

public class Skeleton : Monster
{
    void Update()
    {
        LookAtPlayer();
        SpeedAnimManager();

        if (currentHealth <= 0)
        {
            DeathHandler(2f);
            return;
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        }
    }

    private void SpeedAnimManager()
    {
        if (agent == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Déplacement
        agent.SetDestination(player.position);
        agent.isStopped = distance <= data.attackRange;
        agent.speed = (distance > data.detectionRange) ? moveSpeed * 3f : moveSpeed;

        // Animation
        float animSpeed = (distance > data.detectionRange) ? 2f : 1f;
        anim.SetFloat("Speed", agent.velocity.magnitude > 0f ? animSpeed : 0f);

        // Attaque
        if (distance <= data.attackRange && Time.time - lastAttackTime >= data.attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }
}
