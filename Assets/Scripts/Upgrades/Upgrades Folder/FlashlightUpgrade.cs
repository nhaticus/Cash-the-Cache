using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

public class FlashlightUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    [SerializeField] SingleAudio singleAudio;

    public int price = 60;
    bool purchased = false;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        upgradeInfo.localizeLevel.gameObject.SetActive(false);
        CheckPurchasable();

        if (PlayerPrefs.GetInt("Flashlight") == 1)
        {
            purchased = true;
            PlayerManager.Instance.hasFlashlight = true;
            GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f);
            upgradeInfo.localizeLevel.gameObject.SetActive(true); // show purchased
        }
    }

    public void OnPurchase()
    {
        if (!purchased && GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.hasFlashlight = true;
            GameManager.Instance.SpendMoney(price);
            PlayerPrefs.SetInt("Flashlight", 1);

            upgradeInfo.localizeLevel.gameObject.SetActive(true); // purchased text
            upgradeInfo.shopManager.moneyText.text = "Money: $" + GameManager.Instance.playerMoney.ToString();
            purchased = true;

            singleAudio.PlaySFX("purchase upgrade");
            upgradeInfo.upgradePurchased.Invoke();
            GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f);
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
