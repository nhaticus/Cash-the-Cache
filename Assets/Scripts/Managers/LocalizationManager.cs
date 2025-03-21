using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    public bool active = false;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("SelectedLocale"))
        {
            ChangeLocale(PlayerPrefs.GetInt("SelectedLocale"));
            // active = true;
        }
    }

    public void ChangeLocale(int localeID)
    {
        Debug.Log("Manager: Changing locale to " + localeID);
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