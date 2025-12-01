using System;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Character
{
    public MonsterData data;
    public NavMeshAgent agent;
    public Animator anim;
    public Transform player;
    public MonsterHealthBar healthBarPrefab;
    private MonsterHealthBar healthBarInstance;
    protected float lastAttackTime;



    void Start()
    {
        SetHealthBar();
    }

    public override void DeathHandler(float destructionTime)
    {
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
        HandleDeath(); // vérifie la mort
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


