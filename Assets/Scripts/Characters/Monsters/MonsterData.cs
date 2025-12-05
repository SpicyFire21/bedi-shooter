using UnityEngine;

[CreateAssetMenu(menuName = "Monsters/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("Stats")]
    public float baseMoveSpeed;
    public float baseXpReward;
    public float baseDamage;

    [Header("Comportement")]
    public float detectionRange;
    public float attackRange;
    public float attackCooldown;

    [Header("Spawn")]
    public GameObject prefab;
    public float spawnRate;
    public float maxOnField;
    public float minRangeRadius;
    public float maxRangeRadius;

    [Header("Audio")]
    public AudioClip deathSound;

    public bool onGround;

    private int onField = 0;
    public int monstersOnField => onField;

    public void IncrementOnField() => onField++;
    public void DecrementOnField() => onField = Mathf.Max(0, onField - 1);
    public void ResetOnField() => onField = 0;
}
