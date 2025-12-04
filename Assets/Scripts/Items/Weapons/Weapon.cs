using UnityEngine;

public abstract class Weapon : ItemBase
{
    public bool canDealDamage = false;
    public float weaponDamage;
    public AudioClip actionSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public abstract override void Use(Player player);
}
