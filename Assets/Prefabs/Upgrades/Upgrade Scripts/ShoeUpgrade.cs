using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEditor.Localization.Plugins.XLIFF.V20;

public class ShoeUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    [SerializeField] SingleAudio singleAudio;

    public int price = 40;

    Item runningShoe;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);
        runningShoe = DataSystem.GetOrCreateItem("RunningShoe");
        int level = runningShoe.level;
        if (level > 0)
            price = Mathf.RoundToInt(price * 1.5f * level);
        upgradeInfo.itemPrice.text = "Price: " + price.ToString();
        upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = level.ToString() };
        upgradeInfo.localizeLevel.RefreshString();
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if(GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.increaseMaxMoveSpeed(runningShoe.statValue);
            GameManager.Instance.SpendMoney(price);
            price = Mathf.RoundToInt(price * 1.5f);
            upgradeInfo.itemPrice.text = "Price: " + price.ToString();
            runningShoe.level++;
            DataSystem.SaveItems();
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = runningShoe.level.ToString() };
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
