using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Pause Menu and GameOver spawner
 */
public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject pausePrefab;
    [SerializeField] GameObject gameOverPrefab;
    [SerializeField] TaskListScript taskList;

    private void Start()
    {
        StartCoroutine(FindPlayer());

        // remove task list if played already
        
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
        player.GetComponentInChildren<HealthController>().OnDeath.AddListener(GameOver);
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
        StartCoroutine(CreateGameOverScreen());        

        // unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    float totalFadeInTime = 0.5f;
    IEnumerator CreateGameOverScreen()
    {
        float pauseTimeTillCreation = 0.5f;
        // wait for a little then create game over
        yield return new WaitForSeconds(pauseTimeTillCreation);
        GameObject gameOver = Instantiate(gameOverPrefab, transform); // create game over screen
        gameOver.GetComponent<GameOver>().PlayerLose();

        // fade in canvas
        CanvasGroup gameOverCanvas = gameOver.GetComponent<CanvasGroup>();
        gameOverCanvas.alpha = 0;
        float fadeTimer = 0;
        while (fadeTimer <= totalFadeInTime)
        {
            fadeTimer += Time.deltaTime;
            gameOverCanvas.alpha = fadeTimer / totalFadeInTime;
            yield return null;
        }
        
    }
}
