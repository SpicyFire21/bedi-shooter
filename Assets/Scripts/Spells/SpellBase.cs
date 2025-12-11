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
            UltimateProjectile ultimatePrefab = data.prefab.GetComponent<UltimateProjectile>();

            if (ultimatePrefab != null)
            {
                PlayGlobalSound(ultimatePrefab.data.castSound, data.volume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(data.castSound, this.transform.position, data.volume);
            }
        }
        Cast();
    }

    protected abstract void Cast();

    protected void PlayGlobalSound(AudioClip clip, float volume) // pour play un son en 2D (pour les ultimates, on veut que le son se fasse entendre partout et tout le temps 
        // de la meme maniere)
    {
        GameObject go = new GameObject("GlobalSound");
        AudioSource source = go.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 0f; 
        source.Play();

        Destroy(go, clip.length);
    }

}
