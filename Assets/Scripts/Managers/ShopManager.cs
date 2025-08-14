using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

/*
 * Opens up shop UI
 * Contains list of upgrades that when purchased do upgrading in their own scripts
 */

public class ShopManager : MonoBehaviour
{
    public int numItemsToSpawn = 3;

    public GameObject[] itemsInShop; // list of prefabs for each upgrade

    [Header("UI Panels")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Transform shopPanelTransform;
    public LocalizeStringEvent moneyText;

    [Header("Dependencies")]
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] SingleAudio upgradeSingleAudio;
    
    bool shopActive = false;

    void Start()
    {
        PopulateShop(numItemsToSpawn);

        shopUI.SetActive(false);
    }

    void Update()
    {
        if (shopActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if ((UserInput.Instance && UserInput.Instance.Pause) || (UserInput.Instance == null && Input.GetKeyDown(KeyCode.Escape)))
            {
                CloseShop();
            }
        }
    }

    /// <summary>
    /// Opens shop ui, stops player and populates ui
    /// </summary>
    public void OpenShop()
    {
        if (!shopActive)
        {
            singleAudio.PlaySFX("shop_owner");
            PlayerManager.Instance.ToggleRotation();
            PlayerManager.Instance.ToggleCursor();
            PlayerManager.Instance.setMoveSpeed(0);

            shopUI.SetActive(true);
            shopActive = true;

            moneyText.StringReference["money"] = new StringVariable { Value = GameManager.Instance.playerMoney.ToString() };
            moneyText.RefreshString();
            restockText.StringReference["price"] = new StringVariable { Value = restockPrice.ToString() };
            restockText.RefreshString();
        }
    }

    public void CloseShop()
    {
        PlayerManager.Instance.ToggleRotation();
        PlayerManager.Instance.ToggleCursor();
        PlayerManager.Instance.setMoveSpeed(PlayerManager.Instance.getMaxMoveSpeed());
        shopUI.SetActive(false);
        shopActive = false;
    }

    /// <summary>
    /// Chooses "numItems" random items from itemsInShop
    /// </summary>
    void PopulateShop(int numItems) // fill in shop UI
    {
        int[] itemsChosen = new int[numItems];
        Array.Fill(itemsChosen, -1);

        for (int i = 0; i < numItems; i++)
        {
            int randNum = UnityEngine.Random.Range(0, itemsInShop.Length);

            // check if number exists in itemsChosen
            bool newNum = true;
            while (newNum)
            {
                randNum = UnityEngine.Random.Range(0, itemsInShop.Length);
                newNum = false;
                for (int j = 0; j < itemsChosen.Length; j++)
                {
                    if (itemsChosen[j] == randNum)
                    {
                        newNum = true;
                    }
                }
            }

            itemsChosen[i] = randNum;
            CreateItem(randNum);
        }
        Debug.Log("END populate");
    }

    void CreateItem(int itemNum)
    {
        GameObject item = itemsInShop[itemNum];
        GameObject created = Instantiate(item, shopPanelTransform);
        created.GetComponent<UpgradeInfo>().shopManager = this;
        created.GetComponent<UpgradeInfo>().upgradePurchased.AddListener(CheckUpgrades);
        created.GetComponent<UpgradeInfo>().singleAudio = upgradeSingleAudio; // give single audio reference
    }

    [Header("Restock")]
    [SerializeField] int restockPrice = 15;
    [SerializeField] LocalizeStringEvent restockText;
    public void Restock()
    {
        if (GameManager.Instance.playerMoney >= restockPrice)
        {
            // remove old upgrades
            foreach (Transform child in shopPanelTransform)
            {
                Destroy(child.gameObject);
            }

            PopulateShop(numItemsToSpawn);

            // spend money
            GameManager.Instance.SpendMoney(restockPrice);
            restockPrice += 15;
            restockText.StringReference["price"] = new StringVariable { Value = restockPrice.ToString() };
            restockText.RefreshString();
        }
    }

    void CheckUpgrades()
    {
        foreach (Transform item in shopPanelTransform)
        {
            item.gameObject.GetComponent<UpgradeInfo>().UpdateItem();
        }
    }
    
}
