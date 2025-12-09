[System.Serializable]
public class ItemInstance
{
    public ItemData data;
    public string uniqueID;

    public ItemInstance(ItemData data)
    {
        this.data = data;
        this.uniqueID = System.Guid.NewGuid().ToString();
    }
}
