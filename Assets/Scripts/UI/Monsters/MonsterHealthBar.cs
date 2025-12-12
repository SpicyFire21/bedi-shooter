using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    public Image healthBar;
    public Text levelText;
    public Text healthText;
    public Transform target;

    void LateUpdate() // late update s'appelle après tout les updates de tout les objets
    {
        if (target == null) return;

        float height = GetMonsterHeight(target);

        transform.position = target.position + new Vector3(0, height, 0);
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetHealth(float current, float max)
    {
        healthBar.fillAmount = current / max;
        healthText.text = current.ToString("F1") + " / " + max.ToString("F1");
    }

    public void SetLevel(float level)
    {
        levelText.text = "Lv. " + level.ToString();
    }

    float GetMonsterHeight(Transform monster)
    {
        Collider col = monster.GetComponent<Collider>();
        if (col != null)
        {
            return col.bounds.size.y;
        }

        return 0f; // si pas de collider
    }

}
