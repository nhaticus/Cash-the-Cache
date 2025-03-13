using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using Unity.VisualScripting;

public class Settings : MonoBehaviour
{
    [Header("Language")]
    public LocalizeStringEvent musicLocalizeStringEvent;
    public LocalizeStringEvent SFXLocalizeStringEvent;
    public LocalizeStringEvent sensLocalizeStringEvent;

    [Header("Audio")]
    [SerializeField] TextMeshProUGUI musicText;
    [SerializeField] Slider musicSlider;
    [SerializeField] TextMeshProUGUI SFXText;
    [SerializeField] Slider SFXSlider;
    const float defaultVolume = 1f;
    const float minVolume = 0f;
    const float maxVolume = 1f;

    [Header("Sensitivity")]
    const float defaultSensitivity = 120f;
    const float minSens = 25f;
    const float maxSens = 250f;
    [SerializeField] TextMeshProUGUI sensText;
    [SerializeField] Slider sensSlider;

    private void Start()
    {
        //Music
        float musicVolume = PlayerPrefs.GetFloat("Music", defaultVolume);
        musicSlider.minValue = minVolume;
        musicSlider.maxValue = maxVolume;
        musicSlider.value = musicVolume;
        AudioManager.Instance.MusicVolume(musicVolume);
        UpdateMusicText(Mathf.RoundToInt(musicVolume * 100f));

        //SFX
        float sfxVolume = PlayerPrefs.GetFloat("SFX", defaultVolume);
        SFXSlider.minValue = minVolume;
        SFXSlider.maxValue = maxVolume;
        SFXSlider.value = sfxVolume;
        AudioManager.Instance.SFXVolume(sfxVolume);
        UpdateSFXText(Mathf.RoundToInt(sfxVolume * 100f));

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

    public void SetMusic(float volume)
    {
        AudioManager.Instance.MusicVolume(volume);
        PlayerPrefs.SetFloat("Music", volume);
        UpdateMusicText(Mathf.RoundToInt(volume * 100f));
    }

    public void UpdateMusicText(float volume)
    {
        musicLocalizeStringEvent.StringReference["volumeValue"] = new UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable { Value = volume.ToString("F0") };
        musicLocalizeStringEvent.RefreshString();
    }

    public void SetSFX(float volume)
    {
        AudioManager.Instance.SFXVolume(volume);
        PlayerPrefs.SetFloat("SFX", volume);
        UpdateSFXText(Mathf.RoundToInt(volume * 100f));
    }

    public void UpdateSFXText(float volume)
    {
        SFXLocalizeStringEvent.StringReference["volumeValue"] = new UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable { Value = volume.ToString("F0") };
        SFXLocalizeStringEvent.RefreshString();
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

    public void Reset()
    {
        // Reset Music
        SetMusic(defaultVolume);
        musicSlider.value = defaultVolume;

        // Reset SFX
        SetSFX(defaultVolume);
        SFXSlider.value = defaultVolume;

        // Reset Sensitivity
        SetSensitivity(defaultSensitivity);
        sensSlider.value = defaultSensitivity;
    }

}
