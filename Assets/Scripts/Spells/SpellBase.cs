using UnityEngine;

public abstract class SpellBase : MonoBehaviour
{
    protected Player caster;
    protected SpellData data;
    protected Vector3 destination;

    public virtual void Init(Player player, SpellData spellData, Vector3 dir)
    {
        caster = player;
        data = spellData;
        destination = dir;

        if (data.castSound != null)
        {
            AudioSource.PlayClipAtPoint(data.castSound, transform.position, data.volume);
        }

        Cast();
    }

    protected abstract void Cast();
}
