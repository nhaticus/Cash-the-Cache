using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    public void PlayerLose()
    {
        GameManager.Instance.playerMoney -= 300;
        if (GameManager.Instance.playerMoney < 0)
            GameManager.Instance.playerMoney = 0;
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
}
