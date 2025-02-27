using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Localization.Settings;


public class LocaleSelector : MonoBehaviour
{
    bool active = false;
    public void ChangeLocale(int localeID){
        if(active) return;
        StartCoroutine(SetLocale(localeID));

    }


    IEnumerator SetLocale(int _localeID){
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        // English 0, French 1, Spanish 2
        active = false;
    }
}
