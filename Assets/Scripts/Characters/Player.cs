using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Character
{
    public float maxMana;
    public float manaRegenPerSecond = 0.5f;
    public float healthRegenPerSecond = 1.5f;
    public float currentMana;

    public SpellDatabase spellList;


    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private float meleeRadius = 0.8f;
    [SerializeField] private int meleeDamage = 15;
    [SerializeField] private LayerMask meleeMask;
    public override float MeleeRange => meleeRange;
    public override float MeleeRadius => meleeRadius;
    public override int MeleeDamage => meleeDamage;
    public override LayerMask MeleeMask => meleeMask;

    public Animator animator; // Drag ton Animator dans l’inspecteur


    public override void Attack()
    {
        //Debug.Log("je vais te démarrer fdp");
        //animator.SetTrigger("Attack"); // Déclenche l’animation

        //Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * MeleeRange, MeleeRadius, MeleeMask);

        //foreach (Collider enemy in hitEnemies)
        //{
        //    Character target = enemy.GetComponent<Character>();
        //    if (target != null)
        //    {
        //        target.TakeDamage(MeleeDamage);
        //    }
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {




        HandleDeath();
        if (!isPlayer) {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {   
            Attack();
        }

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
        // borne mini, borne max, si on d�passe = retour a la valeur max
        // cela �vite de d�passer le maxMana et le maxHealth
    }

    public override void DeathHandler(float destructionTime)
    {
        Destroy(gameObject, destructionTime);
    }
}
