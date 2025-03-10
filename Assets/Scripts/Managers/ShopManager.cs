/* Script is attached to the ShopUI prefab
 * This script is used to populate the shop with items from the scriptable object via UpgradeManager
 * It will populate the shop with the item name, description, level, stats, and price
 * To add items, create a new scriptable object and filling it out via the inspector panel then drag it into the items array
 */
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public GameObject shopUI;

    public GameObject itemTemplate;
    public Transform shopPanel;
    private readonly List<GameObject> itemsInShop = new();
    [SerializeField] private TMP_Text moneyText;

    [SerializeField] private TMP_Text openShopPrompt;
    [SerializeField] SingleAudio shopAudio;

    public bool shopActive = false;
    private bool shopOwnerVoicePlayed = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        openShopPrompt.gameObject.SetActive(false);
        shopUI.SetActive(false);
    }
    void Start()
    {
        PopulateShop();
    }

    void Update()
    {
        ShopCheck();
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
            shopAudio.PlaySFX("deny");
            return;
        }
        GameManager.Instance.SpendMoney(itemScriptableObject.price);
        itemScriptableObject.level++;
        itemScriptableObject.price = itemScriptableObject.level * 100;
        UpdateItem(itemScriptableObject, itemGameObject);
        CheckPurchaseable();

        switch (itemScriptableObject.itemName)
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
        DataSystem.SaveItems(UpgradeManager.Instance.loadedItems);
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
                        shopAudio.PlaySFX("shop_owner");
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
}
