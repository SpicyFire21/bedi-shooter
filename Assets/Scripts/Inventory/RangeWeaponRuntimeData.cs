[System.Serializable]
public class RangeWeaponRuntimeData
{
    public ItemInstance instance;
    public int currentAmmo;
    public float lastSoundTime;
    public int ammoInMagazine;
    public int magazineSize;
    public bool initialized = false;
    public float lastShotTime;

    public RangeWeaponRuntimeData(ItemInstance instance, int ammo, float lastSoundTime, int ammoInMagazine, int magazineSize, float lastShotTime)
    {
        this.instance = instance;
        this.currentAmmo = ammo;
        this.lastSoundTime = lastSoundTime;
        this.ammoInMagazine = ammoInMagazine;
        this.magazineSize = magazineSize;
        this.lastShotTime = lastShotTime;
    }
}
