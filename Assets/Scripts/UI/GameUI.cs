using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] GameObject pausePrefab;
    [SerializeField] GameObject gameOverPrefab;
    GameObject pauseRef;
    bool paused = false;

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

    void Pause()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        if (paused)
        {
            pauseRef = Instantiate(pausePrefab, transform);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Destroy(pauseRef);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void GameOver()
    {
        Instantiate(gameOverPrefab, transform);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
