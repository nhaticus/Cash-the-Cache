using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Game Over screen that searches for the player (like PlayerUI.cs)
 * Has commands to show GameOver screen, switch scene, and quit game
 */

public class GameOver : MonoBehaviour
{
    GameObject player;
    PlayerHealth playerHealth;
    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }
        playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.Death.AddListener(ShowGameOver);
    }

    private void ShowGameOver()
    {
        Debug.Log("show canvas");
        GetComponent<CanvasGroup>().alpha = 1;
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
