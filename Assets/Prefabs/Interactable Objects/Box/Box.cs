using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A box object that when clicked on creates a minigame
 * The minigame spawns screws on the screw which need to be clicked to remove
 * When all screws are removed a game object is spawned and the box is destroyed
 */

public class Box : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject[] obj;
    [SerializeField] int difficulty = 4; // difficulty determines amount of clicks (difficulty * 1.7) and number of screws
    [SerializeField] bool setRandomDifficulty = true;
    [SerializeField] int minDifficulty = 3, maxDifficulty = 6;

    [SerializeField] GameObject boxCanvas;
    private void Start()
    {
        if (setRandomDifficulty)
            difficulty = Random.Range(minDifficulty, maxDifficulty);
    }

    // Create difficulty amount of screws and connect their event to ScrewOff
    public void Interact()
    {
       // AnalyticsManager.Instance.TrackMinigameStarted("Box Minigame");
        GameObject canvas = Instantiate(boxCanvas, transform);
        canvas.GetComponent<BoxCanvas>().difficulty = difficulty;
        canvas.GetComponent<BoxCanvas>().OpenBox.AddListener(OpenBox);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);
    }

    void OpenBox()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        // spawn a random object at box position
        Instantiate(obj[Random.Range(0, obj.Length - 1)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
