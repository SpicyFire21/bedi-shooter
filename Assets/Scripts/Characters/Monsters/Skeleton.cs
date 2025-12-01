using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Monster
{
    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (anim == null)
            Debug.LogError("Animator introuvable sur le Skeleton !");
        if (agent == null)
            Debug.LogError("NavMeshAgent introuvable sur le Skeleton !");
    }

    void Update()
    {
        LookAtPlayer();
        SpeedAnimManager();

        if (IsDead())
        {
            DeathHandler(2f);
            return;
        }
    }






    // ===============================
    //          ATTACK
    // ===============================
    private void Attack()
    {
        anim.SetTrigger("Attack");
    }


    private void SpeedAnimManager()
    {
        if (player == null || agent == null || !agent.isOnNavMesh)
        {
            anim.SetFloat("Speed", 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);




        // -----------------------
        // Gérer le déplacement
        // -----------------------
        agent.SetDestination(player.position);

        if (distance <= attackRange)
        {
            // Arrêt devant le joueur
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;

            // Marche / course selon distance
            if (distance > detectionRange)
            {
                agent.speed = moveSpeed * 3f; // course
            }
            else
            {
                agent.speed = moveSpeed;      // marche
            }
        }

        // -----------------------
        // Définir Speed pour l'Animator
        // -----------------------
        float actualSpeed = agent.velocity.magnitude;

        // Seuil minimal pour éviter la marche fantôme
        if (actualSpeed < 0.1f)
            actualSpeed = 0f;

        // Normaliser pour l'Animator (1 = marche, 2 = course)
        float animSpeed = (distance > detectionRange) ? 2f : 1f;
        anim.SetFloat("Speed", (actualSpeed > 0f) ? animSpeed : 0f);

        // -----------------------
        // Attaque
        // -----------------------
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    private void ResetDamageFlag()
    {
        anim.SetBool("Take_damage", false);
    }

    protected override void Die()
    {
    

        if (agent != null)
            agent.isStopped = true;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 5f);
    }

    private bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // on ignore la hauteur pour ne pas pencher le squelette
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Rotation fluide
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // ===============================
    //          TEST ANIMATIONS
    // ===============================
    [ContextMenu("Test Damage Animation")]
    private void TestDamageAnimation()
    {
        TakeDamage(1);
    }

    [ContextMenu("Test Death Animation")]
    private void TestDeathAnimation()
    {
        Die();
    }
}
