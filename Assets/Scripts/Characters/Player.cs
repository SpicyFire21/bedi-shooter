using UnityEngine;
using System.Collections.Generic;

public class Player : Character
{
    public float maxMana;
    public float manaRegenPerSecond = 0.5f;
    public float healthRegenPerSecond = 1.5f;
    public float currentMana;

    public SpellDatabase spellList;


    




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

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
        // borne mini, borne max, si on dépasse = retour a la valeur max
        // cela évite de dépasser le maxMana et le maxHealth
    }
}
