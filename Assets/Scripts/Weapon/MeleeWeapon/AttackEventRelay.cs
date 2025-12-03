using UnityEngine;

public class AttackEventRelay : MonoBehaviour
{
    public PirateSword sword;

    public void EnableDamage()
    {
        if (sword != null)
            sword.EnableDamage();
    }

    public void DisableDamage()
    {
        if (sword != null)
            sword.DisableDamage();
    }
}
