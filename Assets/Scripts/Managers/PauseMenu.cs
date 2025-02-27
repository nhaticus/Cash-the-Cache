using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Pause Menu
 * Same as game over screen but with different title text
 * Seperate script to not clutter GameOver.cs
 */

public class PauseMenu : MonoBehaviour
{
    bool paused = false;

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            GetComponent<CanvasGroup>().alpha = paused ? 1 : 0;
            Time.timeScale = paused ? 0 : 1;
            if (paused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void SwitchScene(string gameScene)
    {
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
