using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 0;
        // check if audio is playing:
        if (AudioManager.Instance && AudioManager.Instance.musicSource.isPlaying)
        {
            AudioManager.Instance.musicSource.Pause();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        if (AudioManager.Instance)
        {
            AudioManager.Instance.musicSource.UnPause();
        }
        UnPause.Invoke();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        Destroy(gameObject);
    }
}
