/* Script is attached to the ShopUI prefab
 * This script is used to populate the shop with items from the scriptable object
 * It will populate the shop with the item name, description, level, stats, and price
 * To add items, create a new scriptable object and filling it out via the inspector panel then drag it into the items array
 */
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public Items[] items;   // Array of items pulled from scriptable object

    // Testing
    public GameObject itemTemplate;
    public Transform shopPanel;
    private List<GameObject> itemsInShop = new List<GameObject>();
    private TMP_Text MoneyText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        MoneyText = GameObject.Find("Money txt").GetComponent<TMP_Text>();
        PopulateShop();
    }

    void Update()
    {
        CheckPurchaseable();
        MoneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();
    }


    public void PopulateShop()
    {
        foreach (Items item in items)
        {
            Items itemScriptableObject = Instantiate(item);
            GameObject itemGameObject = Instantiate(itemTemplate, shopPanel);
            itemsInShop.Add(itemGameObject);
            UpdateItem(itemScriptableObject, itemGameObject);
            itemGameObject.GetComponent<ItemTemplate>().itemData = itemScriptableObject;
            itemGameObject.GetComponent<ItemTemplate>().buyButton.onClick.AddListener(() => BuyItem(itemScriptableObject, itemGameObject));
        }
    }

    private void CheckPurchaseable()
    {
        if (itemsInShop.Count != 0)
        {
            foreach (GameObject item in itemsInShop)
            {
                if (CanBuyItem(item.GetComponent<ItemTemplate>().itemData))
                {
                    item.GetComponent<ItemTemplate>().buyButton.interactable = true;
                }
                else
                {
                    item.GetComponent<ItemTemplate>().buyButton.interactable = false;
                }
            }
        }
    }

    private bool CanBuyItem(Items item)
    {
        return GameManager.Instance.playerMoney >= item.price;
    }

    private void UpdateItem(Items itemScriptableObject, GameObject itemGameObject)
    {
        itemGameObject.GetComponent<ItemTemplate>().itemName.text = itemScriptableObject.item;
        itemGameObject.GetComponent<ItemTemplate>().itemDescription.text = itemScriptableObject.description;
        itemGameObject.GetComponent<ItemTemplate>().itemLevel.text = "Level: " + itemScriptableObject.level.ToString();
        itemGameObject.GetComponent<ItemTemplate>().itemStats.text = itemScriptableObject.stats;
        itemGameObject.GetComponent<ItemTemplate>().itemPrice.text = "Price: " + itemScriptableObject.price.ToString();
    }

    public void BuyItem(Items itemScriptableObject, GameObject itemGameObject)
    {
        GameManager.Instance.playerMoney -= itemScriptableObject.price;
        itemScriptableObject.level++;
        itemScriptableObject.price += itemScriptableObject.level * 100;
        UpdateItem(itemScriptableObject, itemGameObject);

        switch (itemScriptableObject.item)
        {
            case "Backpack":
                UpgradeManager.Instance.upgradeMaxWeight();
                break;
            case "Running Shoes":
                UpgradeManager.Instance.upgradeSpeed();
                break;
            default:
                Debug.Log("Item not found");
                break;
        }
    }

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
