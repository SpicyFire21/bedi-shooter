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
}