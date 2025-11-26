using UnityEngine;
using System.Collections.Generic;

public class Player : Character
{
    public float maxMana = 1000f;
    public float regenPerSecond = 60f;
    public float currentMana = 100f;

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
            currentMana += regenPerSecond * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana;
            Debug.Log(currentMana);        
        }
    }
}
