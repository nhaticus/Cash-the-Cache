using System.Collections;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }
    GameObject player;
    IEnumerator FindPlayer()
    {
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }
        player.GetComponentInChildren<PlayerHealth>().Death.AddListener(GameOver);
    }

    // creates pause menu but doesn't pause
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
        Instantiate(gameOverPrefab, transform);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
