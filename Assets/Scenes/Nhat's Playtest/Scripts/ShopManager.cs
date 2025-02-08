/* Script is attached to the ShopUI prefab
 * This script is used to populate the shop with items from the scriptable object
 * It will populate the shop with the item name, description, level, stats, and price
 * To add items, create a new scriptable object and filling it out via the inspector panel then drag it into the items array
 */
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public Items[] items;   // Array of items pulled from scriptable object
    public ItemTemplate[] itemTemplates;

    void Start()
    {
        populateShop();
    }

    public void populateShop()
    {
        for (int i = 0; i < items.Length; i++)
        {
            itemTemplates[i].gameObject.SetActive(true);
            itemTemplates[i].itemName.text = items[i].item;
            itemTemplates[i].itemDescription.text = items[i].description;
            itemTemplates[i].itemLevel.text = "Level: " + items[i].level.ToString();
            itemTemplates[i].itemStats.text = items[i].stats;
            itemTemplates[i].itemPrice.text = "Price: " + items[i].price.ToString();
        }
    }
}
