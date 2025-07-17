using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class BoxingGloveUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 75;

    Item boxingGloves;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);

        // get current level
        boxingGloves = DataSystem.GetOrCreateItem("BoxingGloves");
        int level = boxingGloves.level;
        if (level > 0) // set price
            price = Mathf.RoundToInt(price * 1.5f * level);

        // set text
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
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
            upgradeInfo.itemPrice.text = "Price: " + price.ToString();

            // increase level
            boxingGloves.level++;
            DataSystem.SaveData();

            // set text
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = boxingGloves.level.ToString() };
            upgradeInfo.localizeLevel.RefreshString();
            upgradeInfo.shopManager.moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();

            upgradeInfo.singleAudio.PlaySFX("purchase upgrade");
            upgradeInfo.upgradePurchased.Invoke();
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
