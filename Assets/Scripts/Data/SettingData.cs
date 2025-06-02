[System.Serializable]
public class AudioSettingsData
{
    // setting default values
    public float masterVolume = 1f; // 1 = 100%
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

}
[System.Serializable]
public class ControlSettingsData
{
    // setting default values
    public float mouseSensitivity = 120f;
    public float controllerSensitivity = 120f;
}

[System.Serializable]
public class GameSettingsData
{
    public AudioSettingsData audio = new();
    public ControlSettingsData controls = new(); 
}