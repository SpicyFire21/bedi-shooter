using UnityEngine;
using UnityEngine.AI;

public class Skeleton : Monster
{
    [Header("AI Settings")]
    public Transform player;

    private NavMeshAgent agent;
    public Animator anim;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

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
        if (IsDead())
            return;

        if (player == null || agent == null || !agent.isOnNavMesh)
        {
            anim.SetFloat("Speed", 0f);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        agent.SetDestination(player.position);

        if (distance > detectionRange)
        {
            // COURT si trop loin
            agent.speed = moveSpeed * 2f;
            anim.SetFloat("Speed", 2f);
        }
        else
        {
            // MARCHE si proche
            agent.speed = moveSpeed;
            anim.SetFloat("Speed", 1f);
        }

        // === attack ===
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // ===============================
    //          ATTACK
    // ===============================
    private void Attack()
    {
        anim.SetTrigger("Attack");
    }

    // ===============================
    //          DAMAGE / DEATH
    // ===============================
    public override void TakeDamage(int amount)
    {
        if (IsDead()) return;

        base.TakeDamage(amount);

        anim.SetBool("Take_damage", true);
        Invoke(nameof(ResetDamageFlag), 0.2f);
    }

    private void ResetDamageFlag()
    {
        anim.SetBool("Take_damage", false);
    }

    protected override void Die()
    {
        anim.SetBool("Is_dead", true);

        if (agent != null)
            agent.isStopped = true;

        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 5f);
    }

    private bool IsDead()
    {
        return anim.GetBool("Is_dead");
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
