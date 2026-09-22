using TMPro;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

public class BackpackUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 40;
    public int maxPrice = 300;

    Item backpack;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);

        backpack = DataSystem.GetOrCreateItem("Backpack");
        int level = backpack.level;
        price = Mathf.Min(Mathf.RoundToInt(price * 1.5f * level), maxPrice);

        upgradeInfo.itemPrice.text = "$" + price.ToString();

        upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = level.ToString() };
        upgradeInfo.localizeLevel.RefreshString();

        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.increaseMaxWeight((int) backpack.statValue);

            GameManager.Instance.SpendMoney(price);
            backpack.level++;
            price = CalculatePrice();
            upgradeInfo.itemPrice.text = "$" + price.ToString();
            DataSystem.SaveData();
            
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = backpack.level.ToString() };
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
        return Mathf.Min(Mathf.RoundToInt(price * 1.5f * backpack.level), maxPrice); ;
    }

    public void CheckPurchasable()
    {
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
