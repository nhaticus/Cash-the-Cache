using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Opens up shop UI
 * Contains list of upgrades that when purchased do upgrading in their own scripts
 */

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] Transform shopPanelTransform;
    [SerializeField] SingleAudio singleAudio;

    public TMP_Text moneyText;

    public GameObject[] itemsInShop; // list of prefabs for each upgrade

    bool shopActive = false;

    void Start()
    {
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

            moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();

            PopulateShop();
        }
    }

    public void CloseShop()
    {
        foreach (Transform obj in shopPanelTransform)
        {
            Destroy(obj.gameObject);
        }

        PlayerManager.Instance.ToggleRotation();
        PlayerManager.Instance.ToggleCursor();
        PlayerManager.Instance.setMoveSpeed(PlayerManager.Instance.getMaxMoveSpeed());
        shopUI.SetActive(false);
        shopActive = false;
    }

    void PopulateShop() // fill in shop UI
    {
        foreach (GameObject item in itemsInShop)
        {
            GameObject created = Instantiate(item, shopPanelTransform);
            created.GetComponent<UpgradeInfo>().shopManager = this;
            created.GetComponent<UpgradeInfo>().upgradePurchased.AddListener(CheckUpgrades);
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
