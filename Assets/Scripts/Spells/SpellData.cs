using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public class SpellData : ScriptableObject
{
    [Header("Identity")]
    public string spellName;
    public Sprite icon;
    public SpellType spellType;

    [Header("Main Values")]
    public float value;           // dégâts, soin, bonus, malus
    public float cooldown;
    public float manaCost;
    public float lifeTime;
    public float effectDuration;

    // [Header("Range & Target")]
    // public bool isTargetedOnGround;
    // public float attackRange;

    [Header("Prefab")]
    public GameObject prefab;

    [Header("Audio")]
    public AudioClip castSound;
    [Range(0f, 1f)] public float volume = 1f;
}
