using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class BoxingGloveUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 75;

    Item vitamins;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);

        // get current level
        vitamins = DataSystem.GetOrCreateItem("Vitamins");
        int level = vitamins.level;
        if (level > 0) // set price
            price = Mathf.RoundToInt(price * 1.5f * level);
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
            price = Mathf.RoundToInt(price * 1.5f);
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

    public void CheckPurchasable()
    {
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
