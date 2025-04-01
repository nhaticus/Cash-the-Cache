using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class UpgradeInfo : MonoBehaviour
{
    [HideInInspector] public NewShopManager shopManager;
    [HideInInspector] public Items itemData;

    public LocalizeStringEvent localizeName;
    public TMP_Text itemPrice;
    public LocalizeStringEvent localizeLevel;
    public LocalizeStringEvent localizeDescription;
    public LocalizeStringEvent localizeStats;
    public Button buyButton;
}
