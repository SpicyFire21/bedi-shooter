using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : Character
{
    public MonsterData data;
    public NavMeshAgent agent;
    public Animator anim;
    protected Transform player;
    public MonsterHealthBar healthBarPrefab;
    private MonsterHealthBar healthBarInstance;
    protected float lastAttackTime;

    public abstract void Attack(float damage);



    public abstract void Spawn(Vector3 spawnPosition);

    protected virtual void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
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


