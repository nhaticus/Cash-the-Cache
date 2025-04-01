using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

/*
 * Opens up shop UI
 * Item Templates have their own purchase code (OnPurchase) which sends message to this to decrease money and show if purchasable
 */

public class NewShopManager : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject itemTemplate;
    public Transform shopPanelTransform;

    List<GameObject> itemsInShop = new List<GameObject>();

    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text openShopPrompt;

    bool shopActive = false;

    void Start()
    {
        openShopPrompt.gameObject.SetActive(false);
        shopUI.SetActive(false);
        PopulateShop();
    }

    void Update()
    {
        if (shopActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        ShopCheck();
    }

    void ShopCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 4))
        {
            if (hit.transform.CompareTag("Shop Keeper"))
            {
                if (!shopActive)
                {
                    openShopPrompt.gameObject.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        OpenShop();
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

    void OpenShop()
    {
        PlayerManager.Instance.ToggleRotation();
        PlayerManager.Instance.ToggleCursor();
        PlayerManager.Instance.setMoveSpeed(PlayerManager.Instance.getMaxMoveSpeed());

        shopUI.SetActive(true);
        openShopPrompt.gameObject.SetActive(false);
        PopulateShop();
    }

    void PopulateShop() // fill in shop UI
    {
        foreach (Items itemScriptableObject in UpgradeManager.Instance.loadedItems)
        {
            GameObject itemGameObject = Instantiate(itemTemplate, shopPanelTransform);
            itemsInShop.Add(itemGameObject);
            InitializeItem(itemScriptableObject, itemGameObject);
            // connect signal to button to change money
        }

    }
    void InitializeItem(Items itemScriptableObject, GameObject itemGameObject)
    {
        ItemTemplate templateComponent = itemGameObject.GetComponent<ItemTemplate>();
        templateComponent.localizeName.SetEntry(itemScriptableObject.itemName);
        templateComponent.localizeLevel.StringReference["level"] = new StringVariable { Value = itemScriptableObject.level.ToString() };
        templateComponent.localizeDescription.SetEntry(itemScriptableObject.itemName + " Description");
        templateComponent.localizeStats.SetEntry(itemScriptableObject.itemName + " Stats");
        templateComponent.itemPrice.text = "Price: " + itemScriptableObject.price.ToString();
    }
}
