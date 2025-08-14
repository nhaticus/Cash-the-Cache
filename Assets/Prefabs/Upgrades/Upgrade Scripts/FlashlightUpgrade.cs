using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class FlashlightUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 60;
    bool purchased = false;
    Item flashlight;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);
        upgradeInfo.itemPrice.text = "$" + price.ToString();

        upgradeInfo.localizeLevel.gameObject.SetActive(false);
        flashlight = DataSystem.GetOrCreateItem("Flashlight");

        // check if flashlight was already bought
        // yes: set display to purchased
        if (flashlight.level == 1)
        {
            purchased = true;
            GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f);
            upgradeInfo.localizeLevel.gameObject.SetActive(true); // show purchased
        }
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (!purchased && GameManager.Instance.playerMoney >= price)
        {
            GameManager.Instance.SpendMoney(price);

            flashlight.level = 1; // set level to 1 mean purchased and 0 means not purchased
            DataSystem.SaveData();
            purchased = true;

            upgradeInfo.localizeLevel.gameObject.SetActive(true); // purchased text
            
            upgradeInfo.PurchaseUpdate();
            GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f);
        }
        else
        {
            upgradeInfo.singleAudio.PlaySFX("deny");
        }
    }
    public void CheckPurchasable()
    {
        if(!purchased)
            GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
