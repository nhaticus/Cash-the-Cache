using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Audio;
using UnityEditor.Localization.Plugins.XLIFF.V20;

/*
 * Changes music, sfx volume and language
 */

public class VolumeSettings : MonoBehaviour
{
    [Header("Language")]
    public LocalizeStringEvent masterLocalizeStringEvent;
    public LocalizeStringEvent musicLocalizeStringEvent;
    public LocalizeStringEvent SFXLocalizeStringEvent;

    [Header("Audio")]
    [SerializeField] TextMeshProUGUI masterText;
    [SerializeField] Slider masterSlider;
    [SerializeField] TextMeshProUGUI musicText;
    [SerializeField] Slider musicSlider;
    [SerializeField] TextMeshProUGUI SFXText;
    [SerializeField] Slider SFXSlider;
    const float defaultVolume = 1f;
    const float minVolume = 0.001f;
    const float maxVolume = 1f;

    [SerializeField] AudioMixer audioMixer;

    AudioSettingsData audioData;

    private void Start()
    {
        audioData = DataSystem.SettingsData.audio;
        //Master
        float masterVolume = audioData.masterVolume;
        masterSlider.minValue = minVolume;
        masterSlider.maxValue = maxVolume;
        masterSlider.value = masterVolume;
        SetMaster(masterVolume);

        //Music
        float musicVolume = audioData.musicVolume;
        musicSlider.minValue = minVolume;
        musicSlider.maxValue = maxVolume;
        musicSlider.value = musicVolume;
        SetMusic(musicVolume);

        //SFX
        float sfxVolume = audioData.sfxVolume;
        SFXSlider.minValue = minVolume;
        SFXSlider.maxValue = maxVolume;
        SFXSlider.value = sfxVolume;
        SetSFX(sfxVolume);
    }

    public void SetMaster(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20f);
        audioData.masterVolume = volume;
        DataSystem.SaveSettings();
        UpdateMasterText(volume);
    }

    public void UpdateMasterText(float volume)
    {
        float adjVol = Mathf.RoundToInt(volume * 100f);
        masterLocalizeStringEvent.StringReference["volumeValue"] = new StringVariable { Value = adjVol.ToString("F0") };
        masterLocalizeStringEvent.RefreshString();
    }

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20f);
        audioData.musicVolume = volume;
        DataSystem.SaveSettings();
        UpdateMusicText(volume);
    }

    public void UpdateMusicText(float volume)
    {
        float adjVol = Mathf.RoundToInt(volume * 100f);
        musicLocalizeStringEvent.StringReference["volumeValue"] = new StringVariable { Value = adjVol.ToString("F0") };
        musicLocalizeStringEvent.RefreshString();
    }

    public void SetSFX(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20f);
        audioData.sfxVolume = volume;
        DataSystem.SaveSettings();
        UpdateSFXText(volume);
    }

    public void UpdateSFXText(float volume)
    {
        float adjVol = Mathf.RoundToInt(volume * 100f);
        SFXLocalizeStringEvent.StringReference["volumeValue"] = new StringVariable { Value = adjVol.ToString("F0") };
        SFXLocalizeStringEvent.RefreshString();
    }

    public void ResetVolume()
    {
        // Reset Master
        SetMaster(defaultVolume);
        masterSlider.value = defaultVolume;

        // Reset Music
        SetMusic(defaultVolume);
        musicSlider.value = defaultVolume;

        // Reset SFX
        SetSFX(defaultVolume);
        SFXSlider.value = defaultVolume;
    }
}
