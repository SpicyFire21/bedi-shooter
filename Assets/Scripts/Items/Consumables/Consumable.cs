using UnityEngine;

public abstract class Consumable : ItemBase
{
    public AudioClip actionSound;
    public float effectValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public abstract void Use(Player player);

    public void Consume(Player player)
    {
        AudioSource.PlayClipAtPoint(actionSound, player.transform.position, 0.5f);
        Use(player);
    }
}
