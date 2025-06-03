using UnityEngine;
using UnityEngine.UI;

public class FlashlightUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    [SerializeField] SingleAudio singleAudio;

    public int price = 60;
    bool purchased = false;
    Item flashlight;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        upgradeInfo.localizeLevel.gameObject.SetActive(false);
        flashlight = DataSystem.GetOrCreateItem("Flashlight");

        // check if flashlight was already bought
        // yes: set display to purchased and set PlayerManager to allow flashlight
        if (flashlight.level == 1)
        {
            purchased = true;
            PlayerManager.Instance.hasFlashlight = true;
            GetComponent<Image>().color = new Color(200f / 255f, 200f / 255f, 200f / 255f);
            upgradeInfo.localizeLevel.gameObject.SetActive(true); // show purchased
        }
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (!purchased && GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.hasFlashlight = true;
            GameManager.Instance.SpendMoney(price);

            flashlight.level = 1; // set level to 1 mean purchased and 0 means not purchased
            DataSystem.SaveItems();

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
        if(!purchased)
            GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
