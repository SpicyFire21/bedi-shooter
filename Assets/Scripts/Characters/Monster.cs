using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : Character
{
    public MonsterData data;
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    public MonsterHealthBar healthBarPrefab;
    private MonsterHealthBar healthBarInstance;
    protected float lastAttackTime;

    
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float meleeRadius = 0.8f;
    [SerializeField] private int meleeDamage = 15;
    [SerializeField] private LayerMask meleeMask;
    public override float MeleeRange => meleeRange;
    public override float MeleeRadius => meleeRadius;
    public override int MeleeDamage => meleeDamage;
    public override LayerMask MeleeMask => meleeMask;

public override void Attack()
    {}



    public abstract void Spawn(Vector3 spawnPosition);

    protected virtual void Start()
    {
        SetHealthBar();
    }


    public override void DeathHandler(float destructionTime)
    {

        data.DecrementOnField();
        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetBool("Is_dead", true);

        if (!deathSoundPlayed && data.deathSound != null)
        {
            AudioSource.PlayClipAtPoint(data.deathSound, transform.position, 1f);
            deathSoundPlayed = true;
        }

        Destroy(gameObject, destructionTime);
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);
    }

    public virtual void Update()
    {
        HandleDeath(); // v�rifie la mort
    }

    public void UpdateHealthBar()
    {
        if (healthBarInstance != null)
            healthBarInstance.SetHealth(currentHealth, maxHealth);
    }

    public void SetHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(
                healthBarPrefab,
                transform.position,
                Quaternion.identity
            );

            healthBarInstance.target = transform;
            healthBarInstance.SetHealth(currentHealth, maxHealth);
        }
    }
}


