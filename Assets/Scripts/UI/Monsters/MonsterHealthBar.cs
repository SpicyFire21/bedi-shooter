using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    public Image healthBar;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);

    void LateUpdate() // late update s'appelle après tout les updates de tout les objets
    {
        if (target == null) return;

        transform.position = target.position + offset;
        transform.rotation = Camera.main.transform.rotation; // billboard --> sert à ce que la barre de vie soit toujours orienté vers NOTRE caméra
    }

    public void SetHealth(float current, float max)
    {
        healthBar.fillAmount = current / max;
    }
}
