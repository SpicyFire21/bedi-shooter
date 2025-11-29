using UnityEngine;

public class Character : MonoBehaviour
{

    [Header("Stats")]
    public float maxHealth;
    public float currentHealth;
    public string characterName;

    [Header("Movement")]
    public float moveSpeed;

    public bool isPlayer; // false par défaut



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
