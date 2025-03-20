using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class LocaleSelector : MonoBehaviour
{
    bool active = false;
    [SerializeField] private TMP_Dropdown dropdown;
    void Awake()
    {
        if (PlayerPrefs.HasKey("SelectedLocale"))
        {
            ChangeLocale(PlayerPrefs.GetInt("SelectedLocale"));
            dropdown.value = PlayerPrefs.GetInt("SelectedLocale");
        }
    }
    public void ChangeLocale(int localeID)
    {
        if (active) return;
        StartCoroutine(SetLocale(localeID));

    }


    IEnumerator SetLocale(int _localeID)
    {
        PlayerPrefs.SetInt("SelectedLocale", _localeID);
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        // English 0, French 1, Spanish 2
        active = false;
    }
}