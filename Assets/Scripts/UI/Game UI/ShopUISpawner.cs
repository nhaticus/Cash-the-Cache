using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUISpawner : MonoBehaviour
{
    [SerializeField] GameObject pausePrefab;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    GameObject pauseRef;
    bool paused = false;
    private void Pause()
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

    void UnPause()
    {
        paused = false;
    }
}
