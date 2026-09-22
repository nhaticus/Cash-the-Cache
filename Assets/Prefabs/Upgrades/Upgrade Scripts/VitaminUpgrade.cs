using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

/*
 * Increases player health
 * Health change is done in PlayerMovement.cs
 * PlayerMovement takes HealthController and increases by vitamins.level * vitamins.statValue
 */

public class VitaminUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 75;
    public int maxPrice = 350;

    Item vitamins;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);

        // get current level
        vitamins = DataSystem.GetOrCreateItem("Vitamins");
        int level = vitamins.level;
        price = CalculatePrice();
        upgradeInfo.itemPrice.text = "$" + price.ToString();

        upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = level.ToString() };
        upgradeInfo.localizeLevel.RefreshString();
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (GameManager.Instance.playerMoney >= price)
        {
            // set price
            GameManager.Instance.SpendMoney(price);
            price = CalculatePrice();
            upgradeInfo.itemPrice.text = "$" + price.ToString();

            // increase level
            vitamins.level++;
            DataSystem.SaveData();

            // set text
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = vitamins.level.ToString() };
            upgradeInfo.localizeLevel.RefreshString();

            upgradeInfo.PurchaseUpdate();

            CheckPurchasable();
        }
        else
        {
            upgradeInfo.singleAudio.PlaySFX("deny");
        }
    }

    int CalculatePrice()
    {
        return Mathf.Min(Mathf.RoundToInt(price * 1.5f * vitamins.level), maxPrice); ;
    }

    public void CheckPurchasable()
    {
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
