using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SpellsPanel : MonoBehaviour
{

    [SerializeField]
    private Player player;

    [SerializeField]
    private Image spellIcon;

    [SerializeField]
    private Text spellName;

    [SerializeField]
    private Text spellKey;

    [SerializeField]
    private Text spellCooldown;

    [SerializeField]
    private GameObject spellPanelPrefab;

    [SerializeField]
    private Transform contentParent;


    private List<GameObject> panelLists;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelLists = new List<GameObject>();
        initSpellsUI();
    }

    // Update is called once per frame
    void Update()
    {
        checkPanels();
        updateSpellsUI();
    }

    private void initSpellsUI()
    {
        for (int i = 0; i < player.spellList.allSpells.Count; i++)
        {
            SpellData spell = player.spellList.allSpells[i];

            // Instancier le panel
            GameObject panel = Instantiate(spellPanelPrefab, contentParent);
            panel.SetActive(true);
            panelLists.Add(panel);

            // recupere les composants UI à l'intérieur du prefab
            Image icon = panel.transform.Find("SpellIcon").GetComponent<Image>();
            Text nameText = panel.transform.Find("SpellName").GetComponent<Text>();
            Text keyText = panel.transform.Find("SpellKey").GetComponent<Text>();
            Text cooldownText = panel.transform.Find("SpellCooldown").GetComponent<Text>();

            // mettre à jour les valeurs
            icon.sprite = spell.icon;
            nameText.text = spell.name;
            //keyText.text = spell.key.ToString();
            cooldownText.text = spell.cooldown.ToString("F1") + "s";
        }
    }

    private void updateSpellsUI()
    { 
        // parce que les deux listes ont la meme taille si bien update --> un panel = un spell et inversement
        for (int i = 0; i < panelLists.Count; i++)
        {
            SpellData spell = player.spellList.allSpells[i];
            Text cooldownText = panelLists[i].transform.Find("SpellCooldown").GetComponent<Text>();
            cooldownText.text = spell.cooldown + "s";
        }
    }

    public void checkPanels()
    {
        // gere le cas ou on a ajouté un spell entre temps et on souhaite donc update les valeurs
        // cela va crée un nouveau panel sans recrée les anciens
        for (int i = panelLists.Count; i < player.spellList.allSpells.Count; i++)
        {
            SpellData spell = player.spellList.allSpells[i];
            GameObject panel = Instantiate(spellPanelPrefab, contentParent);
            panel.SetActive(true);
            panelLists.Add(panel);

            // recupere les composants UI à l'intérieur du prefab
            Image icon = panel.transform.Find("SpellIcon").GetComponent<Image>();
            Text nameText = panel.transform.Find("SpellName").GetComponent<Text>();
            Text keyText = panel.transform.Find("SpellKey").GetComponent<Text>();
            Text cooldownText = panel.transform.Find("SpellCooldown").GetComponent<Text>();

            // mettre à jour les valeurs
            icon.sprite = spell.icon;
            nameText.text = spell.name;
            //keyText.text = spell.key.ToString();
            cooldownText.text = spell.cooldown.ToString("F1") + "s";
        }
    }
}
