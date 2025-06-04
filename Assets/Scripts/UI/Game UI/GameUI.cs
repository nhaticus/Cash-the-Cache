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
        // keep searching for player
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }

        /*  Event Subscribing  */
        player.GetComponentInChildren<HealthController>().OnDeath += GameOver;
    }

    // creates pause menu
    // Game pausing happens in PauseMenu's Start()
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
        GameObject gameOver = Instantiate(gameOverPrefab, transform); // create game over screen

        gameOver.GetComponent<GameOver>().PlayerLose();

        // pause game and unlock cursor
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
