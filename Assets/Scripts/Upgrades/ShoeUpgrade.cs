using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ShoeUpgrade : MonoBehaviour, UpgradeEvent
{
    public NewShopManager shopManager;

    public Items itemData;
    public LocalizeStringEvent localizeName;
    public LocalizeStringEvent localizeLevel;
    public LocalizeStringEvent localizeDescription;
    public LocalizeStringEvent localizeStats;
    public TMP_Text itemPrice;
    public Button buyButton;

    [SerializeField] float moveSpeedUpgradeIncrement = 0.5f;
    public int price = 40;

    public void OnPurchase()
    {
        Debug.Log("Shoe bought");
        PlayerManager.Instance.increaseMoveSpeed(moveSpeedUpgradeIncrement);
        GameManager.Instance.SpendMoney(price);
        price = Mathf.RoundToInt(price * 1.5f);
    }
}
