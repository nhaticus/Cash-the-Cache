using TMPro;
using UnityEngine;

public class ColorblindSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private void Start()
    {
        if (PlayerPrefs.HasKey("ColorblindMode"))
        {
            dropdown.value = PlayerPrefs.GetInt("ColorblindMode");
        }
    }
    public void SelectColorblind(int mode)
    {
        ColorblindToggle.instance.SetColorblindMode(mode);
    }
}
