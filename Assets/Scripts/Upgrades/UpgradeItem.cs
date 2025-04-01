using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class UpgradeItem : MonoBehaviour
{
    public NewShopManager shopManager;

    public Items itemData;
    public LocalizeStringEvent localizeName;
    public LocalizeStringEvent localizeLevel;
    public LocalizeStringEvent localizeDescription;
    public LocalizeStringEvent localizeStats;
    public TMP_Text itemPrice;
    public Button buyButton;
}
