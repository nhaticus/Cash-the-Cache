using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Events;

public class UpgradeInfo : MonoBehaviour
{
    [HideInInspector] public ShopManager shopManager;
    [HideInInspector] public Items itemData;
    [HideInInspector] public UnityEvent upgradePurchased;

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
}
