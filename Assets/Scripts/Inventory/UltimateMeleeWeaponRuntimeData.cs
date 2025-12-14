using UnityEngine;

[System.Serializable]
public class UltimateMeleeWeaponRuntimeData
{
    public ItemInstance instance;
    public float maxRightClickDistance;
    public float rightClickCooldown;
    protected float nextRightClickTime = 0f;
    public LayerMask canRightClickMask;
    public bool initialized = false;

    public UltimateMeleeWeaponRuntimeData(ItemInstance instance, float maxRightClickDistance, float rightClickCooldown, float nextRightClickTime, LayerMask canRightClickMask) 
    {
        this.instance = instance;
        this.maxRightClickDistance = maxRightClickDistance;
        this.rightClickCooldown = rightClickCooldown;
        this.nextRightClickTime = nextRightClickTime;
        this.canRightClickMask = canRightClickMask;
    }
}
