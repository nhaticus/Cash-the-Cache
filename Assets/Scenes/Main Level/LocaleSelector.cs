using TMPro;
using UnityEngine;


public class LocaleSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    void Awake()
    {
        if (PlayerPrefs.HasKey("SelectedLocale"))
        {
            dropdown.value = PlayerPrefs.GetInt("SelectedLocale");
        }
    }
    public void LocalChanged(int localeID)
    {
        Debug.Log("Changing locale to " + localeID);
        LocalizationManager.Instance.ChangeLocale(localeID);
    }
}