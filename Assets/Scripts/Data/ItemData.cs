[System.Serializable]
public class ItemData
{
    public string itemName;
    public string description;
    public int level;
    public string stats;
    public float statValue;
    public int price;

    public ItemData(Items item)
    {
        itemName = item.itemName;
        description = item.description;
        level = item.level;
        stats = item.stats;
        statValue = item.statValue;
        price = item.price;
    }
}
