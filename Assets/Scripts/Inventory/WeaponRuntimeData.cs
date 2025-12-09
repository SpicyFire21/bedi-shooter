[System.Serializable]
public class WeaponRuntimeData
{
    public ItemInstance instance;
    public int currentAmmo;
    public float lastSoundTime;

    public WeaponRuntimeData(ItemInstance instance, int ammo, float lastSoundTime)
    {
        this.instance = instance;
        this.currentAmmo = ammo;
        this.lastSoundTime = lastSoundTime;
    }
}
