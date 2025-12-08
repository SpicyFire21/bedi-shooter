using UnityEngine;

public class Golem : Monster
{
    [Header("Golem Settings")]
    public float stompRange = 3f;       // portée d'attaque au sol
    public float stompCooldown = 2.5f;    // cooldown d’attaque
    public float stompWindup = 0.7f;    // temps de préparation avant dégâts
    public AudioClip stompSound;

    private float lastStompTime;
    private bool isAttacking = false;

    protected override void Start()
    {
        base.Start();

        // Le golem est lent mais puissant
        moveSpeed *= 0.6f;     // 40% plus lent que les autres monstres
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

        // Si en dehors de la portée → il marche vers le joueur
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

    private void TryAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= stompRange && Time.time >= lastStompTime + stompCooldown)
        {
            StartCoroutine(StompRoutine());
        }
    }

    private System.Collections.IEnumerator StompRoutine()
    {
        isAttacking = true;
        agent.isStopped = true;

        anim.SetTrigger("Stomp");

        // Son optionnel au début du coup
        if (stompSound != null)
            AudioSource.PlayClipAtPoint(stompSound, transform.position);

        // Temps avant que l’impact touche
        yield return new WaitForSeconds(stompWindup);

        DealStompDamage();

        // Fin de l’animation → petit délai
        yield return new WaitForSeconds(0.4f);

        lastStompTime = Time.time;
        isAttacking = false;
    }

    private void DealStompDamage()
    {
        // Détection des cibles autour du Golem
        Collider[] hits = Physics.OverlapSphere(transform.position, stompRange);

        foreach (Collider hit in hits)
        {
            Player playerCharacter = hit.GetComponent<Player>();

            if (playerCharacter != null)
            {
                // Le golem frappe fort → dégâts multipliés par 2
                playerCharacter.TakeDamage(damage * 2f);
            }
        }
    }

    public override void Attack(float value) { }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}
