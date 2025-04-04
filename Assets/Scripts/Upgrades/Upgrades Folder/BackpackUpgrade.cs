using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    [SerializeField] int upgradeIncrement = 3;
    public int price = 40;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();

        price = Mathf.RoundToInt((1.5f * PlayerPrefs.GetInt("Backpack")) + price);
        // change level text

        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
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

            GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
        }
        else
        {
            AudioManager.Instance.PlaySFX("deny");
        }
    }
}
