using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;

    void Start()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        settingsMenu.SetActive(false);
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

    [HideInInspector] public UnityEvent UnPause;
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
