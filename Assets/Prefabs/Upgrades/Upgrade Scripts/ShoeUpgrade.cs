using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class ShoeUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

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
        upgradeInfo.itemPrice.text = "$" + price.ToString();

        upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = level.ToString() };
        upgradeInfo.localizeLevel.RefreshString();
        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if(GameManager.Instance.playerMoney >= price)
        {
            PlayerManager.Instance.setMaxMoveSpeed(runningShoe.statValue * runningShoe.level);
            GameManager.Instance.SpendMoney(price);
            price = Mathf.RoundToInt(price * 1.5f);
            upgradeInfo.itemPrice.text = "$" + price.ToString();

            runningShoe.level++;
            DataSystem.SaveData();
            upgradeInfo.localizeLevel.StringReference["level"] = new StringVariable { Value = runningShoe.level.ToString() };
            upgradeInfo.localizeLevel.RefreshString();

            upgradeInfo.PurchaseUpdate();
            CheckPurchasable();
        }
        else
        {
            upgradeInfo.singleAudio.PlaySFX("deny");
        }
    }

    public void CheckPurchasable()
    {
        GetComponent<Image>().color = GameManager.Instance.playerMoney < price ? new Color(200f / 255f, 200f / 255f, 200f / 255f) : Color.white;
    }
}
