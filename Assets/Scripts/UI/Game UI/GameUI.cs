using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Pause Menu and GameOver spawner
 */
public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject pausePrefab;
    [SerializeField] GameObject gameOverPrefab;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        if ((UserInput.Instance && UserInput.Instance.Pause) || (UserInput.Instance == null && Input.GetKeyDown(KeyCode.P)))
        {
            Pause();
        }
    }

    // connect OnDeath event to Game Over
    GameObject player;
    IEnumerator FindPlayer()
    {
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }
        player.GetComponentInChildren<HealthController>().OnDeath += GameOver;
    }

    // creates pause menu
    // pausing happens in PauseMenu.cs on Start()
    GameObject pauseRef;
    bool paused = false;
    void Pause()
    {
        paused = !paused;
        if (paused)
        {
            pauseRef = Instantiate(pausePrefab, transform);
            pauseRef.GetComponentInChildren<PauseMenu>().UnPause.AddListener(UnPause);
        }
        else
        {
            pauseRef.GetComponent<PauseMenu>().ReturnToGame();
        }
    }

    void UnPause() // Return to Game in Pause Menu
    {
        paused = false;
    }

    void GameOver()
    {
        GameObject gameOver = Instantiate(gameOverPrefab, transform);
        gameOver.GetComponent<GameOver>().PlayerLose();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
