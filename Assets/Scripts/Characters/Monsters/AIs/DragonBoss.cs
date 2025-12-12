using UnityEngine;
using UnityEngine.AI;

public class DragonBoss : Monster
{
    public enum DragonState
    {
        Ground,
        Flying,
        TransitionToFly,
        TransitionToGround,
        Dead
    }

    [Header("Dragon Settings")]
    public DragonState state = DragonState.Ground;

    [Header("Ground Phase")]
    public float groundAttackRange = 4f;
    public float groundAttackCooldown = 3f;
    public AudioClip groundRoarSound;

    [Header("Flight Phase")]
    public float flyHeight = 15f;
    public float flySpeed = 12f;
    public float aerialAttackCooldown = 5f;
    public float fireballRange = 30f;
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;

    [Header("Transitions")]
    public float takeoffDuration = 2f;
    public float landingDuration = 2f;

    private float lastGroundAttackTime;
    private float lastAerialAttackTime;
    private bool isTransitioning = false;

    private NavMeshAgent agent;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        // Le dragon commence au sol
        state = DragonState.Ground;
        agent.enabled = true;
    }

    public override void Update()
    {
        if (isDead || state == DragonState.Dead) return;
        base.Update();

        switch (state)
        {
            case DragonState.Ground:
                GroundLogic();
                break;

            case DragonState.Flying:
                FlyingLogic();
                break;

            case DragonState.TransitionToFly:
            case DragonState.TransitionToGround:
                // Animations jouent, on attend
                break;
        }
    }
    private bool isAttacking = false;
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

    // --------------------------
    //     GROUND LOGIC
    // --------------------------
    private void GroundLogic()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Déplacement
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetFloat("Speed", agent.velocity.magnitude);

        // Attaque au sol
        if (distance <= groundAttackRange)
        {
            if (Time.time > lastGroundAttackTime + groundAttackCooldown)
            {
                GroundAttack();
            }
        }

        // Condition pour voler (ex: 50% HP)
        if (currentHealth <= maxHealth * 0.5f)
        {
            StartFlyingTransition();
        }
    }

    private void GroundAttack()
    {
        lastGroundAttackTime = Time.time;
        agent.isStopped = true;
        anim.SetTrigger("GroundAttack");
    }

    // Animation Event
    public void DealGroundDamage()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= groundAttackRange)
        {
            player.GetComponent<Player>()?.TakeDamage(damage * 1.5f);
        }
    }

    // --------------------------
    //     FLYING LOGIC
    // --------------------------
    private void FlyingLogic()
    {
        if (player == null) return;

        // Le dragon vole autour du joueur
        Vector3 targetPos = player.position + (transform.right * 10f);
        targetPos.y = flyHeight;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * flySpeed);
        LookAtPlayer();

        // Aerial attack
        if (Time.time > lastAerialAttackTime + aerialAttackCooldown)
        {
            AerialAttack();
        }

        // Quand HP est faible → il revient au sol pour phase finale
        if (currentHealth <= maxHealth * 0.2f)
        {
            StartLandingTransition();
        }
    }

    private void AerialAttack()
    {
        lastAerialAttackTime = Time.time;
        anim.SetTrigger("AerialAttack");
    }

    // Animation Event
    public void LaunchFireball()
    {
        if (fireballPrefab == null || fireballSpawnPoint == null) return;

        GameObject fb = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
        // Ajoute un script de fireball avec déplacement + explosion plus tard
    }

    // --------------------------
    //     TRANSITIONS
    // --------------------------
    private void StartFlyingTransition()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        state = DragonState.TransitionToFly;
        agent.isStopped = true;
        agent.enabled = false;

        anim.SetTrigger("TakeOff");

        Invoke(nameof(FinishTakeoff), takeoffDuration);
    }

    private void FinishTakeoff()
    {
        state = DragonState.Flying;
        isTransitioning = false;
    }

    private void StartLandingTransition()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        state = DragonState.TransitionToGround;

        anim.SetTrigger("Land");

        Invoke(nameof(FinishLanding), landingDuration);
    }

    private void FinishLanding()
    {
        agent.enabled = true;
        state = DragonState.Ground;
        isTransitioning = false;
    }

    // --------------------------
    //     DEATH
    // --------------------------
    public override void DeathHandler(float destructionTime)
    {
        base.DeathHandler(destructionTime);

        state = DragonState.Dead;

        // Disable movement
        if (agent != null)
            agent.enabled = false;

        anim.SetTrigger("Death");
    }

    public override void Attack(float damage)
    {
        
    }

    public override void Spawn(Vector3 spawnPosition, int level)
    {
        data.IncrementOnField();
        SetLevel(level);
        transform.position = spawnPosition;
    }
}
