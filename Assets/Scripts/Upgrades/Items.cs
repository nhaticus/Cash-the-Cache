/* Generic class that holds the information of the items in the shop. */
using UnityEngine;

[CreateAssetMenu(fileName = "shopItem", menuName = "Shop Scriptable Object/New Item")]
public class Items : ScriptableObject
{
    public string itemName;
    public string description;
    public int level;
    public string stats;
    public int price;
}
