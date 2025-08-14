using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class UpgradeInfo : MonoBehaviour
{
    [HideInInspector] public ShopManager shopManager;
    [HideInInspector] public UnityEvent upgradePurchased;
    public SingleAudio singleAudio;

    public LocalizeStringEvent localizeName;
    public TMP_Text itemPrice;
    public LocalizeStringEvent localizeLevel;
    public LocalizeStringEvent localizeDescription;
    public LocalizeStringEvent localizeStats;

    [HideInInspector] public UnityEvent updateItem;

    public void UpdateItem()
    {
        updateItem.Invoke();
    }

    /// <summary>
    /// Generic function to update money text, play sound, and invoke updateItem event
    /// </summary>
    public void PurchaseUpdate()
    {
        shopManager.moneyText.StringReference["money"] = new StringVariable { Value = GameManager.Instance.playerMoney.ToString() };
        shopManager.moneyText.RefreshString();

        singleAudio.PlaySFX("purchase upgrade");
        upgradePurchased.Invoke();
    }
}
