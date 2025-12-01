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
    public bool deathSoundAlreadyDone = false;
}
