using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpellDatabase", menuName = "Spells/Spell Database")]
public class SpellDatabase : ScriptableObject
{
    public List<SpellData> allSpells = new List<SpellData>();
}