using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : Character
{
    [Header("Data")]
    public MonsterData data;

    [Header("Level System")]
    [SerializeField] protected int level = 1;

    [Header("Components")]
    public NavMeshAgent agent;
    public Animator anim;

    protected Transform player;
    protected float lastAttackTime;

    [Header("Combat Stats (Calculated)")]
    protected float damage;

    [Header("UI")]
    public MonsterHealthBar healthBarPrefab;
    private MonsterHealthBar healthBarInstance;

    
    protected virtual void Start()
    {
        FindPlayer();
        ApplyStats();
        SetHealthBar();
    }

    protected void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }


    // applique les stats selon le niveau
    protected virtual void ApplyStats()
    {
        float healthMultiplier = 1f + (level - 1) * 0.3f;
        float damageMultiplier = 1f + (level - 1) * 0.18f;

        maxHealth = baseMaxHealth * healthMultiplier;
        currentHealth = maxHealth;

        damage = data.baseDamage * damageMultiplier;

        if (agent != null)
            agent.speed = moveSpeed;
    }

    public abstract void Attack(float damage);
    public abstract void Spawn(Vector3 spawnPosition, int level);

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

        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                float xpReward = CalculateXPReward();
                playerScript.GainXP(xpReward);
            }
        }

        Destroy(gameObject, destructionTime);

        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);
    }

    public virtual void Update()
    {
        HandleDeath();
    }
    public void UpdateHealthBar()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.SetHealth(currentHealth, maxHealth);
            healthBarInstance.SetLevel(level);
        }
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
            healthBarInstance.SetLevel(level);
        }
    }


    // pour gerer le level
    public void SetLevel(int newLevel)
    {
        level = Mathf.Max(1, newLevel);
        ApplyStats();
        UpdateHealthBar();
    }

    private float CalculateXPReward()
    {
        // XP croît avec le niveau du monstre
        // Exemple : base XP * (1 + 30% par niveau au-dessus du niveau 1)
        float xp = data.baseXpReward * (1f + 0.3f * (level - 1));


        return Mathf.Clamp(xp, data.baseXpReward * 0.5f, data.baseXpReward * 20f); // on evite les gains enormes d'xp quand meme
    }

}
