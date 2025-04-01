using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 60;
    bool purchased = false;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }

    public void OnPurchase()
    {
        if (!purchased && GameManager.Instance.playerMoney >= price)
        {
            UpgradeManager.Instance.SetFlashlight(true);
            GameManager.Instance.SpendMoney(price);
            // change text to purchased

            upgradeInfo.shopManager.moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();
            purchased = true;

            GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f);
        }
        else
        {
            AudioManager.Instance.PlaySFX("deny");
        }
    }
}
