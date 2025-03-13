using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
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
        musicText.text = $"Music Volume: {musicVolume * 100f:0}%";

        //SFX
        float sfxVolume = PlayerPrefs.GetFloat("SFX", defaultVolume);
        SFXSlider.minValue = minVolume;
        SFXSlider.maxValue = maxVolume;
        SFXSlider.value = sfxVolume;
        AudioManager.Instance.SFXVolume(sfxVolume);
        SFXText.text = $"SFX Volume: {sfxVolume * 100f:0}%";

        //Sensitivity
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
        sensSlider.minValue = minSens;
        sensSlider.maxValue = maxSens;
        if (sensitivity > maxSens)
            sensitivity = maxSens;
        else if (sensitivity < minSens)
            sensitivity = minSens;
        sensSlider.value = sensitivity;
        sensText.text = $"Sensitivity: {sensitivity / 25:0.00}";
    }

    public void SetMusic(float volume)
    {
        AudioManager.Instance.MusicVolume(volume);
        PlayerPrefs.SetFloat("Music", volume);
        musicText.text = $"Music Volume: {volume * 100f:0}%";
    }

    public void SetSFX(float volume)
    {
        AudioManager.Instance.SFXVolume(volume);
        PlayerPrefs.SetFloat("SFX", volume);
        SFXText.text = $"SFX Volume: {volume * 100f:0}%";
    }
    public void SetSensitivity(float sensitivity)
    {
        PlayerManager.Instance.SetSensitivity(sensitivity);
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        sensText.text = $"Sensitivity: {sensitivity / 25:0.00}";
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
