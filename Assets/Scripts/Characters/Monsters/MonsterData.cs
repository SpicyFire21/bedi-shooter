using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Monsters/Monster Data")]
public class MonsterData : ScriptableObject
{
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public GameObject prefab;
    public float attackCooldown = 1.5f;
    public float lastAttackTime;
    public AudioClip deathSound;    // son joué quand le sort est lancé
    public float spawnRate;
    public float maxOnField;
    private int onField = 0;
    public int monstersOnField => onField; // toute cette logique evite de pouvoir directement modifié depuis l'inspecteur ce qui pourrait casser le jeu
    public void IncrementOnField() => onField++;
    public void DecrementOnField() => onField = Mathf.Max(0, onField - 1);

    public void ResetOnField() => onField = 0;
}
