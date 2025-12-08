using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class Player : Character
{
    [Header("Base Stats")]
    public float baseMaxMana = 20f;
    public float baseDamage = 10f;
    public float baseManaRegenPerSecond = 0.3f;
    public float baseHealthRegenPerSecond = 0.6f;

    // ces bases stats vont servir poue le scaling du personnage --> on va toujours se baser sur les stats qu'il avait de base pour pas qu'il devienne trop OP 

    [Header("Final Stats")]
    [HideInInspector] public float maxMana;
    [HideInInspector] public float currentMana;
    [HideInInspector] public float damage;
    [HideInInspector] public float manaRegenPerSecond;
    [HideInInspector] public float healthRegenPerSecond;

    [Header("Spells")]
    public SpellDatabase spellList;

    [Header("Animator")]
    public Animator animator;

    [Header("XP System")]
    public int level = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;
    public float xpMultiplierPerLevel = 1.2f;

    [Header("Equipment Slots")]
    public Transform headSlot;
    public Transform chestSlot;
    public Transform weaponSlot;
    public Transform legsSlot;
    public Transform bootsSlot;

    [Header("Equipment Bonus")]
    [HideInInspector] public float bonusDamage;
    [HideInInspector] public float bonusMaxHealth;
    [HideInInspector] public float bonusMoveSpeed;


    private Weapon equippedWeapon; // l'arme actuellement équipée

    public ThirdPersonController tps;
    private Monster attackTarget;


    void Start()
    {
        ApplyStats();

        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    void Update()
    {
        if (tps.MoveSpeed != moveSpeed)
        {
            tps.MoveSpeed = moveSpeed;
            tps.SprintSpeed = moveSpeed * 1.5f;
        }

        HandleDeath();

        if (!isPlayer) return;

        if (currentMana < maxMana)
        {
            currentMana += manaRegenPerSecond * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana;
        }

        if (currentHealth < maxHealth)
        {
            currentHealth += healthRegenPerSecond * Time.deltaTime;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        currentMana = Mathf.Clamp(currentMana, 0f, maxMana);


        if (Time.timeScale == 0f) return; // bloque toute attaque si le jeu est en pause
        Debug.Log("temps chec");
        if (Inventory.instance.IsOpen()) return;
        Debug.Log("deuxieme chec");
        if (Input.GetMouseButtonDown(0) && equippedWeapon != null)
        {
            equippedWeapon.Attack(this);
        }
    }

    public override void DeathHandler(float destructionTime)
    {
        Destroy(gameObject, destructionTime);
    }

    // on va appeler cette methode a chaque fois que l'on tue un monstre
    public void GainXP(float amount)
    {
        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel *= xpMultiplierPerLevel;
        ApplyStats();
        Debug.Log("Level up adam est une pomme irl: " + level);
    }

    public virtual void ApplyStats()
    {
        float healthMultiplier, manaMultiplier, damageMultiplier, regenMultiplier;
        // calcule les dégâts du joueur en combinant une augmentation linéaire par niveau et une croissance exponentielle
        // en gros, on veut que plus on est haut niveau, plus les valeurs sont grande sans être abusé quand même

        // exemple : si level = 10, Mathf.Pow(10, 1.15) ≈ 14.1, *0.015 ≈ 0.2115, +1 → damageMultiplier ≈ 1.2115
        // Donc le joueur inflige 21% de dégâts supplémentaires par rapport à baseDamage.
        if (level != 1)
        {
             healthMultiplier = 1f + Mathf.Pow(level, 1.15f) * 0.08f;
             manaMultiplier = 1f + Mathf.Pow(level, 1.12f) * 0.05f;
             damageMultiplier = 1f + Mathf.Pow(level, 1.15f) * 0.015f;
             regenMultiplier = 1f + Mathf.Pow(level, 1.10f) * 0.02f;
        } else
        {
            healthMultiplier = 1f;
            manaMultiplier = 1f;
            damageMultiplier = 1f;
            regenMultiplier = 1f;
        }

        // BASE + EQUIPEMENTS + LEVEL
        maxHealth = (baseMaxHealth * healthMultiplier) + bonusMaxHealth;
        Debug.Log("affichage test : " + bonusMaxHealth);
        maxMana = baseMaxMana * manaMultiplier;
        damage = (baseDamage * damageMultiplier) + bonusDamage;

        manaRegenPerSecond = baseManaRegenPerSecond * regenMultiplier;
        healthRegenPerSecond = baseHealthRegenPerSecond * regenMultiplier;

        moveSpeed = baseMoveSpeed + bonusMoveSpeed;

        tps.MoveSpeed = moveSpeed;
    }


    // pour les armes
    public void SetEquippedWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    public void SetAttackTarget(Monster target)
    {
        attackTarget = target;
    }

    public void ApplyWeaponDamage()
    {
        if (attackTarget != null)
            attackTarget.TakeDamage(equippedWeapon.weaponDamage);
    }

    public void OnAttackEnd()
    {
        tps.canMove = true;
    }



}
