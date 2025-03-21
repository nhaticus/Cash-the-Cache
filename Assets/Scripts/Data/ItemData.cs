[System.Serializable]
public class ItemData
{
    public string itemName;
    public string description;
    public int level;
    public float statValue;
    public int price;

    public ItemData(Items item)
    {
        itemName = item.itemName;
        level = item.level;
        statValue = item.statValue;
        price = item.price;
    }
}
