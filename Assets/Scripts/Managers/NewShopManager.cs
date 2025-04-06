using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Opens up shop UI
 * Contains list of upgrades that when purchased do upgrading in their own scripts
 */

public class NewShopManager : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] Transform shopPanelTransform;
    [SerializeField] SingleAudio singleAudio;

    public TMP_Text moneyText;
    [SerializeField] TMP_Text openShopPrompt;

    public GameObject[] itemsInShop; // list of prefabs for each upgrade

    bool voicePlayed = false;
    bool shopActive = false;

    void Start()
    {
        openShopPrompt.gameObject.SetActive(false);
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
        else
        {
            ShopCheck();
        }
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
                    if (!voicePlayed) // play voice once
                    {
                        singleAudio.PlaySFX("shop_owner");
                        voicePlayed = true;
                    }

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
        PlayerManager.Instance.setMoveSpeed(0);

        shopUI.SetActive(true);
        openShopPrompt.gameObject.SetActive(false);
        shopActive = true;

        moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();

        PopulateShop();
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
        foreach (GameObject upgrade in shopPanelTransform)
        {
            //upgrade.GetComponent<>().CheckPurchasable();
        }
    }
    
}
