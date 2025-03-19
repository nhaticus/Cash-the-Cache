using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

/*
 * Changes music, sfx volume and language
 */

public class VolumeSettings : MonoBehaviour
{
    [Header("Language")]
    public LocalizeStringEvent musicLocalizeStringEvent;
    public LocalizeStringEvent SFXLocalizeStringEvent;

    [Header("Audio")]
    [SerializeField] TextMeshProUGUI musicText;
    [SerializeField] Slider musicSlider;
    [SerializeField] TextMeshProUGUI SFXText;
    [SerializeField] Slider SFXSlider;
    const float defaultVolume = 1f;
    const float minVolume = 0f;
    const float maxVolume = 1f;

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
    public void ResetVolume()
    {
        // Reset Music
        SetMusic(defaultVolume);
        musicSlider.value = defaultVolume;

        // Reset SFX
        SetSFX(defaultVolume);
        SFXSlider.value = defaultVolume;
    }
}
