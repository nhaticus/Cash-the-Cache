using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class ScrewDriverUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    [SerializeField] SingleAudio singleAudio;

    [SerializeField] float upgradeIncrement = 0.3f;
    public int price = 50;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);
        int level = PlayerPrefs.GetInt("Backpack");
        if (level > 0)
            price = Mathf.RoundToInt(price * 1.5f * level);
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = level.ToString() };
        upgradeInfo.localizeLevel.RefreshString();
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.IncreaseBoxOpening(upgradeIncrement);
            GameManager.Instance.SpendMoney(price);
            price = Mathf.RoundToInt(price * 1.5f);
            upgradeInfo.itemPrice.text = "Price: " + price.ToString();
            PlayerPrefs.SetInt("Screwdriver", PlayerPrefs.GetInt("Screwdriver") + 1);
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = PlayerPrefs.GetInt("Screwdriver").ToString() };
            upgradeInfo.localizeLevel.RefreshString();

            upgradeInfo.shopManager.moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();

            singleAudio.PlaySFX("purchase upgrade");
            upgradeInfo.upgradePurchased.Invoke();
            CheckPurchasable();
        }
        else
        {
            singleAudio.PlaySFX("deny");
        }
    }

    public void CheckPurchasable()
    {
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
