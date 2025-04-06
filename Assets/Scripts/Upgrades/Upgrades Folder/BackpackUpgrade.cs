using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    [SerializeField] SingleAudio singleAudio;

    [SerializeField] int upgradeIncrement = 3;
    public int price = 40;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        price = Mathf.RoundToInt((1.5f * PlayerPrefs.GetInt("Backpack")) + price);
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        // change level text
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.increaseMaxWeight(upgradeIncrement);
            GameManager.Instance.SpendMoney(price);
            price = Mathf.RoundToInt(price * 1.5f);
            upgradeInfo.itemPrice.text = "Price: " + price.ToString();
            PlayerPrefs.SetInt("Backpack", PlayerPrefs.GetInt("Backpack") + 1);
            // change level text

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
