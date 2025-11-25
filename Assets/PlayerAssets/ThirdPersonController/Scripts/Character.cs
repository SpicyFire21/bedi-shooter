using UnityEngine;

public class Character : MonoBehaviour
{

    [Header("Stats")]
    public float maxHealth = 20f;
    public float currentHealth = 20f;
    public string characterName;

    [Header("Movement")]
    public float moveSpeed = 5f;

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
