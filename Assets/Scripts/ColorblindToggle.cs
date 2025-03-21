using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class ColorblindToggle : MonoBehaviour
{
    public static ColorblindToggle instance; // Singleton for persistence
    public Volume globalVolume; // Active Volume in the scene
    public VolumeProfile defaultProfile;
    public VolumeProfile deuteranopiaProfile;
    public VolumeProfile tritanopiaProfile;
    public VolumeProfile achromatopsiaProfile;

    private string colorblindModeKey = "ColorblindMode"; // Save key

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Apply saved colorblind mode at start
        int mode = PlayerPrefs.GetInt(colorblindModeKey, 0);
        SetColorblindMode(mode);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) SetColorblindMode(0); // Default
        if (Input.GetKeyDown(KeyCode.F2)) SetColorblindMode(1); // Deuteranopia
        if (Input.GetKeyDown(KeyCode.F3)) SetColorblindMode(2); // Tritanopia
        if (Input.GetKeyDown(KeyCode.F4)) SetColorblindMode(3); // Achromatopsia
    }

    public void SetColorblindMode(int mode)
    {
        // Swap the active Volume Profile
        switch (mode)
        {
            case 0:
                globalVolume.profile = defaultProfile;
                break;
            case 1:
                globalVolume.profile = deuteranopiaProfile;
                break;
            case 2:
                globalVolume.profile = tritanopiaProfile;
                break;
            case 3:
                globalVolume.profile = achromatopsiaProfile;
                break;
        }

        // Save the selected mode
        PlayerPrefs.SetInt(colorblindModeKey, mode);
        PlayerPrefs.Save();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find and assign the new scene's Global Volume
        globalVolume = FindObjectOfType<Volume>();
        if (globalVolume != null)
        {
            // Reapply the saved colorblind mode in the new scene
            SetColorblindMode(PlayerPrefs.GetInt(colorblindModeKey, 0));
        }
    }
}