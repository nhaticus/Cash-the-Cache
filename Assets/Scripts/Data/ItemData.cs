using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;
    public int level;
    public float statValue;
}

[System.Serializable]
public class ItemData
{
    public List<Item> items = new();
}