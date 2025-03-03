using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject settingsPanel;
    [Header("Audio")]
    public TextMeshProUGUI musicText;
    public Slider musicSlider;
    public TextMeshProUGUI SFXText;
    public Slider SFXSlider;
    private const float defaultVolume = 1f;
    private const float minVolume = 0f;
    private const float maxVolume = 1f;

    [Header("Sensitivity")]
    private const float defaultSensitivity = 120f;
    private const float minSens = 10f;
    private const float maxSens = 1000f;
    public TextMeshProUGUI sensText;
    public Slider sensSlider;

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
        sensSlider.value = sensitivity;
        sensText.text = $"Sensitivity: {sensitivity / 100:0.00}";
        settingsPanel.SetActive(false);
    }

    public void SetMusic(float volume)
    {
        // Debug.Log($"Music: {volume}");
        AudioManager.Instance.MusicVolume(volume);
        PlayerPrefs.SetFloat("Music", volume);
        musicText.text = $"Music Volume: {volume * 100f:0}%";
    }

    public void SetSFX(float volume)
    {
        // Debug.Log($"SFX: {volume}");
        AudioManager.Instance.SFXVolume(volume);
        PlayerPrefs.SetFloat("SFX", volume);
        SFXText.text = $"SFX Volume: {volume * 100f:0}%";
    }
    public void SetSensitivity(float sensitivity)
    {
        // Debug.Log($"Sensitivity: {sensitivity}");
        PlayerManager.Instance.SetSensitivity(sensitivity);
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        sensText.text = $"Sensitivity: {sensitivity / 100:0.00}";
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
