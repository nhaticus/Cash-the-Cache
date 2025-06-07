using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject settingsMenu;

    [SerializeField]
    GameObject firstPauseButton,
        firstSettingsButton;

    void Start()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ToggleSettings(false);
    }

    public void ToggleSettings(bool isToggled)
    {
        settingsMenu.SetActive(isToggled);

        GameObject selectedButton = isToggled ? firstSettingsButton : firstPauseButton;
        EventSystem.current.SetSelectedGameObject(selectedButton);
    }

    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    [HideInInspector]
    public UnityEvent UnPause;

    public void ReturnToGame()
    {
        // Play sound again:
        UnPause.Invoke();

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        Destroy(gameObject);
    }
}
