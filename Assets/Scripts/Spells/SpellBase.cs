using UnityEngine;

public abstract class SpellBase : MonoBehaviour
{
    public Character caster;
    protected SpellData data;

    public virtual void Init(Character character, SpellData spellData)
    {
        Debug.Log("Appel slt");
        caster = character;
        data = spellData;

        if (data.castSound != null)
        {
            AudioSource.PlayClipAtPoint(data.castSound, transform.position, data.volume);
        }

        Cast();
    }

    protected abstract void Cast();
}
