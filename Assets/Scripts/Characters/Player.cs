using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class Player : Character
{
    [Header("Base Stats")]
    public float baseMaxMana = 20f;
    public float baseDamage = 10f;
    public float baseManaRegenPerSecond = 0.555f;
    public float baseHealthRegenPerSecond = 1.2f;

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

    [Header("Others")]
    public ThirdPersonController tps;
    private Monster attackTarget;
    public LayerMask notPlayerMask;


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
        if (Inventory.instance.IsOpen()) return;

        if (equippedWeapon != null && tps.Grounded)
        {
            // si c'est un RangeWeapon, on peut laissé appuyer pour tirer
            if (equippedWeapon is RangeWeapon)
            {
                if (Input.GetKeyDown(KeyCode.R) && ((equippedWeapon as RangeWeapon).getAmmoInMagazine() != (equippedWeapon as RangeWeapon).magazineSize))
                {
                    (equippedWeapon as RangeWeapon).StartReload(this);
                }
                if (Input.GetMouseButton(0) && !(equippedWeapon as RangeWeapon).IsReloading())
                {
                    FaceMouse();
                    equippedWeapon.Attack(this);
                } else if (Input.GetMouseButtonUp(0))
                {
                    (equippedWeapon as RangeWeapon).StopAnimation(this);
                }
            }
            else // arme de mêlée
            {
                if (Input.GetMouseButtonDown(0))
                {
                    FaceMouse();
                    equippedWeapon.Attack(this);
                }
                if (Input.GetMouseButtonDown(1) && equippedWeapon is UltimateMeleeWeapon)
                {
                    (equippedWeapon as UltimateMeleeWeapon).DoRightClick(this);
                } else if (Input.GetKeyDown(KeyCode.V) && equippedWeapon is UltimateMeleeWeapon)
                {
                    (equippedWeapon as UltimateMeleeWeapon).SpecialAbility(this);
                }
            }
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

        if (level != 1)
        {
            healthMultiplier = 1f + Mathf.Pow(level, 1.15f) * 0.08f;
            manaMultiplier = 1f + Mathf.Pow(level, 1.12f) * 0.05f;
            damageMultiplier = 1f + Mathf.Pow(level, 1.15f) * 0.05f;
            regenMultiplier = 1f + Mathf.Pow(level, 1.10f) * 0.02f;
        }
        else
        {
            healthMultiplier = 1f;
            manaMultiplier = 1f;
            damageMultiplier = 1f;
            regenMultiplier = 1f;
        }

        // sauvegarde le pourcentage de vie actuel
        float healthPercent = maxHealth > 0 ? currentHealth / maxHealth : 1f;

        // calcule les nouvelles stats (niveau + equipement)
        maxHealth = (baseMaxHealth * healthMultiplier) + bonusMaxHealth;
        maxMana = baseMaxMana * manaMultiplier;
        damage = (baseDamage * damageMultiplier) + bonusDamage;

        manaRegenPerSecond = baseManaRegenPerSecond * regenMultiplier;
        healthRegenPerSecond = baseHealthRegenPerSecond * regenMultiplier;

        moveSpeed = baseMoveSpeed + bonusMoveSpeed;

        // quand on equipe un equipement, on regenere un peu de point de vie 
        currentHealth = maxHealth * healthPercent;
        currentMana = maxMana * (currentMana / maxMana); // pour le mana

        tps.MoveSpeed = moveSpeed;
        tps.SprintSpeed = moveSpeed * 1.85f;
    }



    // pour les armes
    public void SetEquippedWeapon(Weapon weapon)
    {
        equippedWeapon = weapon;
    }

    public Weapon GetEquippedWeapon()
    {
        return equippedWeapon;
    }

    public UltimateMeleeWeapon GetUltimateMeleeWeapon()
    {
        return equippedWeapon as UltimateMeleeWeapon;
    }

    public void SetAttackTarget(Monster target)
    {
        attackTarget = target;
    }

    public void ApplyWeaponDamage()
    {
        if (attackTarget != null)
        {
            // si les degats de l'arme est une valeur inferieur au damage du personnage / 2, on met les degats de damage + weapondamage / 2 pour éviter au personnage d'être trop pénaliser
            // si il a une arme de très mauvaise qualité et un niveau et un equipement de niveau élevé
            if (equippedWeapon is MeleeWeapon)
            {
                if (equippedWeapon.weaponDamage >= (damage / 1.8))
                {
                    attackTarget.TakeDamage(equippedWeapon.weaponDamage);
                }
                else
                {
                    attackTarget.TakeDamage((equippedWeapon.weaponDamage + damage) / 2);
                }

                if (equippedWeapon is UltimateMeleeWeapon)
                {
                    (equippedWeapon as UltimateMeleeWeapon).ImpactEffect(this, attackTarget);
                }
            } else if (equippedWeapon is RangeWeapon)
            {
                if (equippedWeapon.weaponDamage >= (damage / 2))
                {
                    attackTarget.TakeDamage(equippedWeapon.weaponDamage);
                }
                else
                {
                    attackTarget.TakeDamage((equippedWeapon.weaponDamage + damage) / 8.5f);
                }
            }
        }
    }

    public void OnAttackEnd()
    {
        tps.canMove = true;
    }

    protected virtual Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, notPlayerMask)) // TOUT sauf le player
        {
            return hit.point; // retourne la vraie position dans le monde
        }

        // fallback : point très loin dans la direction du rayon
        return ray.origin + ray.direction * 100f;
    }

    public void FaceMouse()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 direction = mouseWorldPos - transform.position;
        direction.y = 0; // on ignore la hauteur

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation; 
                                              
        }
    }





}
