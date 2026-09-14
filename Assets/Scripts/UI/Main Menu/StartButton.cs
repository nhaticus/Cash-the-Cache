using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    [SerializeField] string scene = "Map";
    [SerializeField] GameObject cutscene;

    public void OnClick() {
        if (DataSystem.Data.gameState.currentReplay > 0)
        {
            SwitchScene(scene);
        }
        else
        {
            Instantiate(cutscene);
        }
    }

    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }
}
