using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ControlSettings : MonoBehaviour
{
    [Header("Language")]
    public LocalizeStringEvent sensLocalizeStringEvent;

    [Header("Sensitivity")]
    const float defaultSensitivity = 120f;
    const float minSens = 25f;
    const float maxSens = 250f;
    [SerializeField] TextMeshProUGUI sensText;
    [SerializeField] Slider sensSlider;

    private void Start()
    {
        //Sensitivity
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
        sensSlider.minValue = minSens;
        sensSlider.maxValue = maxSens;
        if (sensitivity > maxSens)
            sensitivity = maxSens;
        else if (sensitivity < minSens)
            sensitivity = minSens;
        sensSlider.value = sensitivity;
        UpdateSensitivityText(sensitivity);
    }

    public void SetSensitivity(float sensitivity)
    {
        PlayerManager.Instance.SetSensitivity(sensitivity);
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        UpdateSensitivityText(sensitivity);
    }
    public void UpdateSensitivityText(float sensitivity)
    {
        sensLocalizeStringEvent.StringReference["sensitivityValue"] = new UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable { Value = (sensitivity / 25).ToString("F2") };
        sensLocalizeStringEvent.RefreshString();
    }

    public void ResetControls()
    {
        // Reset Sensitivity
        SetSensitivity(defaultSensitivity);
        sensSlider.value = defaultSensitivity;
    }
}
