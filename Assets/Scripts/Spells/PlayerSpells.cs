using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Apple;

// structure pour associer une touche à un sort dans playerdatabase
[System.Serializable]
public struct SpellKey
{
    public KeyCode key;      // touche pour lancer le sort
    public int spellIndex;   // index du sort dans playerdatabase.allSpells
}

public class PlayerSpells : MonoBehaviour
{
    public SpellDatabase playerDatabase; // base de données des sorts débloqués pour ce joueur
    public Transform castPoint;          // point depuis lequel le sort sera lancé
    public List<SpellKey> spellKeys;     // mapping touches -> index des sorts

    private Player player;        // référence au script player pour gérer le mana etc
    private float[] cooldowns;    // tableau pour gérer les cooldowns de chaque sort
    private Vector3 destination;
    public LayerMask groundLayer;

    void Start()
    {
        player = GetComponent<Player>(); // on récupère le script player attaché au même gameobject

        if (playerDatabase != null)
            cooldowns = new float[playerDatabase.allSpells.Count]; // initialise le tableau de cooldowns
    }

    void Update()
    {
        // on diminue les cooldowns à chaque frame
        for (int i = 0; i < cooldowns.Length; i++)
            cooldowns[i] -= Time.deltaTime;

        // on vérifie si une touche de sort est pressée
        foreach (var sk in spellKeys)
        {
            if (Input.GetKeyDown(sk.key))
                TryCastSpell(sk.spellIndex);
        }
    }

    // méthode pour lancer un sort par son index
    public void TryCastSpell(int index)
    {
        Debug.Log("tentative spell");
        GameObject obj;
        if (index >= playerDatabase.allSpells.Count) return;
        if (cooldowns[index] > 0f) return;

        SpellData spell = playerDatabase.allSpells[index];

        if (player.currentMana < spell.manaCost) return;

        if (spell.isTargetedOnGround)
        {
            destination = GetMouseGroundPosition(spell.attackRange);
            if (destination == Vector3.zero)
            {
                Debug.Log("pas de sol pour ce spell");
                return; // on bloque le lancement si pas de sol
            }
            obj = Instantiate(
                spell.prefab,
                destination,
                Quaternion.identity
            );
        }
        else
        {
            destination = GetMouseWorldPosition();
            obj = Instantiate(
                spell.prefab,
                castPoint.position,
                Quaternion.identity
            );
        }

        
        SpellBase spellLogic = obj.GetComponent<SpellBase>();

        if (spellLogic != null)
        {
            spellLogic.Init(player, spell, destination);
            player.currentMana -= spell.manaCost;
            cooldowns[index] = spell.cooldown;
        }

        
    }

    // méthode pour débloquer un nouveau sort à la volée
    public void UnlockSpell(SpellData spell, KeyCode key)
    {
        playerDatabase.allSpells.Add(spell); // ajoute le sort à la base
        spellKeys.Add(new SpellKey { key = key, spellIndex = playerDatabase.allSpells.Count - 1 }); // mapping touche->index
        System.Array.Resize(ref cooldowns, playerDatabase.allSpells.Count); // ajuste le tableau de cooldowns
        Debug.Log("sort débloqué : " + spell.spellName + " sur la touche " + key);
    }

    // récupère la position de la souris dans le monde
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // ignore le joueur et renvoie un point loin devant
                Vector3 fallback = ray.GetPoint(1000);
                return fallback;
            }
            return hit.point;
        }
        else
        {
            Vector3 fallback = ray.GetPoint(1000);
            return fallback;
        }
    }

    private Vector3 GetMouseGroundPosition(float maxRange)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, groundLayer)) // si c'est pas un groundLayer, on ne peut PAS instancier cette objet
        {
            float dist = Vector3.Distance(player.transform.position, hit.point);

            if (dist <= maxRange)
                return hit.point;

            Vector3 dir = (hit.point - player.transform.position).normalized;
            return player.transform.position + dir * maxRange;
        }

        return Vector3.zero;
    }

    public float GetCooldown(int index)
    {
        if (index >= 0 && index < cooldowns.Length)
            return Mathf.Max(0f, cooldowns[index]);
        return 0f;
    }

    public KeyCode GetKeyForSpell(int spellIndex)
    {
        foreach (var sk in spellKeys)
        {
            if (sk.spellIndex == spellIndex)
                return sk.key;
        }

        return KeyCode.None;
    }







}
