using UnityEngine;
using UnityEngine.UI;

public class WhoopeeCushionUpgrade : MonoBehaviour
{
    UpgradeInfo upgradeInfo;

    public int price = 10;

    [SerializeField] string[] sounds;

    private void Start()
    {
        upgradeInfo = GetComponent<UpgradeInfo>();
        upgradeInfo.updateItem.AddListener(CheckPurchasable);
        upgradeInfo.itemPrice.text = "$" + price.ToString();

        CheckPurchasable();
    }

    public void OnPurchase()
    {
        if (GameManager.Instance.playerMoney >= price)
        {
            GameManager.Instance.SpendMoney(price);
            DataSystem.SaveData();

            // play random whoopee cushion sound
            upgradeInfo.singleAudio.PlaySFX(sounds[Random.Range(0, sounds.Length)]);
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
