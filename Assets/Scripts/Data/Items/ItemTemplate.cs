/*
 * Attached to each UI shop item prefab to access the text components
 */
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ItemTemplate : MonoBehaviour
{
    public Items itemData;
    public LocalizeStringEvent localizeName;
    public LocalizeStringEvent localizeLevel;
    public LocalizeStringEvent localizeDescription;
    public LocalizeStringEvent localizeStats;
    public TMP_Text itemPrice;
    public Button buyButton;

}
