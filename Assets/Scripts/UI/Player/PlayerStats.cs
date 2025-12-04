using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    [SerializeField]
    private Player player;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private Image manaBarFill;

    [SerializeField]
    private TextMeshProUGUI healthBarText;

    [SerializeField]
    private TextMeshProUGUI manaBarText;

    [SerializeField]
    private Image xpBar;

    [SerializeField]
    private Text levelText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // le fillamount est une valeur sur 1, pour récuperer une valeur équivalente il faut donc
        // effectuer cette opération
        healthBarFill.fillAmount = player.currentHealth / player.maxHealth;
        manaBarFill.fillAmount = player.currentMana / player.maxMana;
        healthBarText.text = player.currentHealth.ToString("F1") + " / " + player.maxHealth.ToString("F1");
        manaBarText.text = player.currentMana.ToString("F1") + " / " + player.maxMana.ToString("F1");
        xpBar.fillAmount = player.currentXP / player.xpToNextLevel;
        levelText.text = "Lv. " + player.level.ToString();
        // F1 = un chiffre après la virgule
    }
}
