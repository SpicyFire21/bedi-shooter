using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public class SpellData : ScriptableObject
{
    public string spellName;
    public Sprite icon;
    public float damage;
    public float cooldown;
    public GameObject prefab;   // l'objet à instancier
    public float manaCost;
    public bool isTargetedOnGround; // si le sort doit être lancé au sol
    public float attackRange;

    [Header("Audio")]
    public AudioClip castSound;    // son joué quand le sort est lancé
    //public AudioClip impactSound;    // son joué quand le sort touche quelque chose

    [Range(0f, 1f)] public float volume = 1f; // volume du son

}