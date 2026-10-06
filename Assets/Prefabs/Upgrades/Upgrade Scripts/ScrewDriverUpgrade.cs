using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class ScrewDriverUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 50;
    public int maxPrice = 250;
    [SerializeField] float priceMult = 0.35f;

    Item screwdriver;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);

        screwdriver = DataSystem.GetOrCreateItem("Screwdriver");
        int level = screwdriver.level;
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
            GameManager.Instance.SpendMoney(price);
            price = CalculatePrice();
            upgradeInfo.itemPrice.text = "$" + price.ToString();

            screwdriver.level++;
            DataSystem.SaveData();
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = screwdriver.level.ToString() };
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
        int adjustedPrice = Mathf.RoundToInt(price + (price * priceMult * screwdriver.level));
        return Mathf.Min(adjustedPrice, maxPrice);
    }


    public void CheckPurchasable()
    {
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
