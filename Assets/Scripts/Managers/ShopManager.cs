/* Script is attached to the ShopUI prefab
 * This script is used to populate the shop with items from the scriptable object via UpgradeManager
 * It will populate the shop with the item name, description, level, stats, and price
 * To add items, create a new scriptable object and filling it out via the inspector panel then drag it into the items array
 */
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class ShopManager : MonoBehaviour
{
    // public static ShopManager Instance { get; private set; }

    public GameObject shopUI;

    public GameObject itemTemplate;
    public Transform shopPanel;
    private readonly List<GameObject> itemsInShop = new();
    [SerializeField] private TMP_Text moneyText;

    [SerializeField] private TMP_Text openShopPrompt;
    SingleAudio singleAudio;

    public bool shopActive = false;
    private bool shopOwnerVoicePlayed = false;


    void Awake()
    {
        openShopPrompt.gameObject.SetActive(false);
        shopUI.SetActive(false);
    }
    void Start()
    {
        UpgradeManager.Instance.SetFlashlight(false);
        PopulateShop();

        singleAudio = GetComponent<SingleAudio>();
    }

    void Update()
    {
        if (shopActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        ShopCheck();
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            ResetShop();
        }
    }


    public void PopulateShop()
    {
        foreach (Items itemScriptableObject in UpgradeManager.Instance.loadedItems)
        {
            GameObject itemGameObject = Instantiate(itemTemplate, shopPanel);
            itemsInShop.Add(itemGameObject);
            UpdateItem(itemScriptableObject, itemGameObject);
            ItemTemplate templateComponent = itemGameObject.GetComponent<ItemTemplate>();
            templateComponent.itemData = itemScriptableObject;
            templateComponent.buyButton.onClick.AddListener(() => BuyItem(itemScriptableObject, itemGameObject));
        }
        UpdateMoneyText();
        CheckPurchaseable();
    }

    private void CheckPurchaseable() // toggling item buttons to be interactable if have enough money
    {
        if (itemsInShop.Count != 0)
        {
            foreach (GameObject item in itemsInShop)
            {
                item.GetComponent<Image>().color = CanBuyItem(item.GetComponent<ItemTemplate>().itemData) ? Color.white : new Color(200f / 255f, 200f / 255f, 200f / 255f);
            }
        }
    }

    private bool CanBuyItem(Items item)
    {
        return GameManager.Instance.playerMoney >= item.price;
    }

    private void UpdateItem(Items itemScriptableObject, GameObject itemGameObject)
    {
        ItemTemplate templateComponent = itemGameObject.GetComponent<ItemTemplate>();
        templateComponent.itemName.text = itemScriptableObject.itemName;
        templateComponent.itemDescription.text = itemScriptableObject.description;
        templateComponent.itemLevel.text = "Level: " + itemScriptableObject.level.ToString();
        templateComponent.itemStats.text = itemScriptableObject.stats;
        templateComponent.itemPrice.text = "Price: " + itemScriptableObject.price.ToString();
    }

    public void BuyItem(Items itemScriptableObject, GameObject itemGameObject)
    {
        if (!CanBuyItem(itemScriptableObject))
        {
            singleAudio.PlaySFX("deny");
            return;
        }
        GameManager.Instance.SpendMoney(itemScriptableObject.price);
        
        switch (itemScriptableObject.itemName)
        {
            case "Backpack":
                UpgradeManager.Instance.upgradeMaxWeight();
                itemScriptableObject.level++;
                itemScriptableObject.price = Mathf.RoundToInt(itemScriptableObject.price * 1.5f);
                break;
            case "Running Shoes":
                UpgradeManager.Instance.upgradeSpeed();
                itemScriptableObject.level++;
                itemScriptableObject.price = Mathf.RoundToInt(itemScriptableObject.price * 1.5f);
                break;
            case "Screwdriver":
                UpgradeManager.Instance.UpgradeScrewdriver(0.5f);
                itemScriptableObject.level++;
                itemScriptableObject.price = Mathf.RoundToInt(itemScriptableObject.price * 1.5f);
                break;
            case "Flashlight":
                UpgradeManager.Instance.SetFlashlight(true);
                ItemTemplate templateComponent = itemGameObject.GetComponent<ItemTemplate>();
                templateComponent.itemStats.text = "purchased";
                templateComponent.buyButton.interactable = false;
                break;
            default:
                Debug.Log("Item not found");
                break;
        }
        DataSystem.SaveItems(UpgradeManager.Instance.loadedItems);

        UpdateItem(itemScriptableObject, itemGameObject);
        UpdateMoneyText();
    }
    private void ShopCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 4))
        {
            if (hit.transform.CompareTag("Shop Keeper"))
            {
                if (!shopActive)
                {
                    // stop voiceline from being played mulitple times:
                    if (!shopOwnerVoicePlayed)
                    {
                        singleAudio.PlaySFX("shop_owner");
                        shopOwnerVoicePlayed = true;
                    }

                    openShopPrompt.gameObject.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        ToggleShop();
                        PlayerManager.Instance.setMoveSpeed(0);
                        UpdateMoneyText();
                        openShopPrompt.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                openShopPrompt.gameObject.SetActive(false);
            }
        }
        else
        {
            openShopPrompt.gameObject.SetActive(false);
        }
    }

    public void ToggleShop()
    {
        shopActive = !shopActive;
        PlayerManager.Instance.ToggleRotation();
        PlayerManager.Instance.ToggleCursor();
        PlayerManager.Instance.setMoveSpeed(PlayerManager.Instance.getMaxMoveSpeed());
        shopUI.SetActive(!shopUI.activeSelf);
        CheckPurchaseable();
    }

    private void UpdateMoneyText()
    {
        CheckPurchaseable();
        moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();
    }

    private void ResetShop()
    {
        PlayerPrefs.DeleteAll();
        UpgradeManager.Instance.ResetData();
        GameManager.Instance.numRuns = 0;
        foreach (GameObject item in itemsInShop)
        {
            Destroy(item);
        }
        itemsInShop.Clear();

        PopulateShop();
    }
}
