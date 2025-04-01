using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

/*
 * Opens up shop UI
 * Item Templates have their own purchase code (OnPurchase) which sends message to this to decrease money and show if purchasable
 */

public class NewShopManager : MonoBehaviour
{
    [SerializeField] GameObject shopUI;
    [SerializeField] Transform shopPanelTransform;

    public TMP_Text moneyText;
    [SerializeField] TMP_Text openShopPrompt;

    public GameObject[] itemsInShop; // list of prefabs for each upgrade

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
        PlayerManager.Instance.setMoveSpeed(0);

        shopUI.SetActive(true);
        openShopPrompt.gameObject.SetActive(false);
        shopActive = true;

        moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();

        PopulateShop();
    }

    void PopulateShop() // fill in shop UI
    {
        foreach (GameObject item in itemsInShop)
        {
            GameObject created = Instantiate(item, shopPanelTransform);
            // connect signal to button to change money
            created.GetComponent<UpgradeInfo>().shopManager = this;
            
        }

    }
}
