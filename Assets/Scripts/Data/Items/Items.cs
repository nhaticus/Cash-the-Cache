/* Generic class that holds the information of the items in the shop. */
using UnityEngine;

[CreateAssetMenu(fileName = "shopItem", menuName = "Shop Scriptable Object/New Item")]
public class Items : ScriptableObject
{
    public string itemName;
    public int level;
    public float statValue;
    public int price;

    public void Initialize(ItemData item)
    {
        itemName = item.itemName;
        level = item.level;
        statValue = item.statValue;
        price = item.price;
    }
}
