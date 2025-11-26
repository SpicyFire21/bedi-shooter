using UnityEngine;
using System.Collections.Generic;

// structure pour associer une touche à un sort dans PlayerSpellDatabase
[System.Serializable]
public struct SpellKey
{
    public KeyCode key;      // touche pour lancer le sort
    public int spellIndex;   // index dans PlayerSpellDatabase.allSpells
}

public class PlayerSpells : MonoBehaviour
{
    public SpellDatabase playerDatabase; // la base des sorts débloqués pour ce joueur
    public Transform castPoint;                // point depuis lequel les sorts seront lancés
    public List<SpellKey> spellKeys;           // mapping touches -> index dans playerDatabase

    private Player player;        // référence au joueur pour accéder au mana, etc.
    private float[] cooldowns;    // tableau des cooldowns pour chaque sort

    void Start()
    {
        // on récupère le script player attaché au même gameobject
        player = GetComponent<Player>();

        // on initialise le tableau de cooldowns en fonction du nombre de sorts débloqués
        if (playerDatabase != null)
            cooldowns = new float[playerDatabase.allSpells.Count];
    }

    void Update()
    {
        // on réduit le temps restant des cooldowns chaque frame
        for (int i = 0; i < cooldowns.Length; i++)
            cooldowns[i] -= Time.deltaTime;

        // on parcourt toutes les touches assignées aux sorts
        foreach (var sk in spellKeys)
        {
            // si la touche est pressée, on essaye de lancer le sort correspondant
            if (Input.GetKeyDown(sk.key))
                TryCastSpell(sk.spellIndex);
            
        }
    }

    // méthode pour lancer un sort par son index dans playerDatabase
    public void TryCastSpell(int index)
    {
        Debug.Log("tentative de sort");
        if (playerDatabase == null) return;                       // sécurité : database inexistante
        if (index >= playerDatabase.allSpells.Count) return;     // sécurité : index invalide
        if (cooldowns[index] > 0f) return;                       // sort en cooldown, on ne fait rien

        SpellData spell = playerDatabase.allSpells[index];       // on récupère les infos du sort

        // vérification du mana
        if (player.currentMana < spell.manaCost)
        {
            Debug.Log("mana insuffisant pour " + spell.spellName);
            return;
        }

        // on dépense le mana du joueur
        player.currentMana -= spell.manaCost;

        // instanciation du prefab du sort à la position et rotation du castPoint
        GameObject obj = Instantiate(spell.prefab, castPoint.position, castPoint.rotation);

        // appel générique de Init si le prefab a une méthode Init (voir exemple pour la fireball par exemple)
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            var method = s.GetType().GetMethod("Init");
            if (method != null)
                method.Invoke(s, new object[] { player }); // on passe le joueur au prefab

            // pour faire clair, on va appeler sans connaitre la methode init de la prefab selectionné (elle prend un character
            // en param, donc on va y assigner le player actuel)
        }

        // on met le sort en cooldown après le lancer
        cooldowns[index] = spell.cooldown;

        // debug pour confirmer le lancement
        Debug.Log("sort lancé : " + spell.spellName);
    }

    // méthode pour débloquer un nouveau sort à la volée
    public void UnlockSpell(SpellData spell, KeyCode key)
    {
        // on ajoute le sort à la PlayerSpellDatabase
        playerDatabase.allSpells.Add(spell);

        // on ajoute le mapping touche -> index
        spellKeys.Add(new SpellKey { key = key, spellIndex = playerDatabase.allSpells.Count - 1 });

        // on agrandit le tableau de cooldowns pour ce nouveau sort
        System.Array.Resize(ref cooldowns, playerDatabase.allSpells.Count);

        // debug pour confirmer le déblocage
        Debug.Log("sort débloqué : " + spell.spellName + " sur la touche " + key);
    }
}
