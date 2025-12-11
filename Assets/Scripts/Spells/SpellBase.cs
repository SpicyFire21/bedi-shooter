using UnityEngine;

public abstract class SpellBase : MonoBehaviour
{
    public Character caster;
    protected SpellData data;
    protected float localValue; // on va utiliser cette variable pour les calculs de scales par rapport au niveau du personnage

    public virtual void Init(Character character, SpellData spellData)
    {
        caster = character;
        data = spellData;

        if (data.castSound != null)
        {
            AudioSource.PlayClipAtPoint(data.castSound, this.transform.position, data.volume);
        }
        (caster as Player).FaceMouse();
        Cast();
    }

    protected abstract void Cast();
}
