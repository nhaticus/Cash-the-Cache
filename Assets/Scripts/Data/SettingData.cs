[System.Serializable]
public class AudioSettingsData
{
    // setting default values
    public float masterVolume = 1f; // 1 = 100%
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

}

[System.Serializable]
public class KeyBindData
{
    // work in process not sure how keybinds are stored
}

[System.Serializable]
public class ControllerSettingsData
{
    public float controllerSensitivity = 120f; // default sensitivity for mouse
    public KeyBindData keyBinds = new(); // default key binds
}

[System.Serializable]
public class KeyboardSettingsData
{
    public float mouseSensitivity = 120f; // default sensitivity for mouse
    public KeyBindData keyBinds = new(); // default key binds
}

[System.Serializable]
public class GameSettingsData
{
    public AudioSettingsData audio = new();
    public ControllerSettingsData controller = new();
    public KeyboardSettingsData keyboard = new();
}