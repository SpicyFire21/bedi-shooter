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

    public Animator animator; 


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
