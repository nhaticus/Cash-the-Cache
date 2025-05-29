using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A Toilet interactable object
 * Keep flushing the toilet and keep the plunger in the correct position
 */

public class Toilet : MonoBehaviour, InteractEvent
{
    [Header("Toilet Settings")]
    public bool isUnlocked = false;

    [Header("Difficulty = Time it takes to flush")]
    [SerializeField] int difficulty = 3; // Difficulty level = number of seconds x 1
    [SerializeField] bool setRandomDifficulty;
    [SerializeField] int minDifficulty = 3, maxDifficulty = 6;

    public bool isFlushingOpen = false;  // Whether the flushing mini-game is open or not

    [Header("Canvas Settings")]
    [SerializeField] GameObject FlushingCanvasPrefab; // Prefab for the flushing UI canvas
    private GameObject currentCanvas; // Reference to the currently active canvas

    [Header("Loot Settings")]
    [SerializeField] List<GameObject> loot;
    [SerializeField] Transform spawnPos;

    private void Start()
    {
        if (setRandomDifficulty)
            difficulty = Random.Range(minDifficulty, maxDifficulty);
    }

    // This method is called when the player interacts with the toilet
    public void Interact()
    {
        //if(AnalyticsManager.Instance)
        //    AnalyticsManager.Instance.TrackMinigameStarted("Toliet Minigame");
        if (isFlushingOpen)
        {
            Debug.Log("Already interacting with the flushing minigame.");
            return;  // If flushing is already open, don't allow further interaction
        }

        isFlushingOpen = true;

        ResetToilet();

        // Check if the canvas already exists and is active
        if (currentCanvas == null) 
            InitializeFlushingCanvas();
        else
            currentCanvas.SetActive(true);

        LockPlayerControls();
    }

    private void InitializeFlushingCanvas()
    {
        currentCanvas = Instantiate(FlushingCanvasPrefab, transform);
        currentCanvas.SetActive(true);
        FlushingCanvas canvasScript = currentCanvas.GetComponent<FlushingCanvas>();

        // Set the difficulty for mini-game and add listeners for success
        canvasScript.difficulty = difficulty;
        canvasScript.toiletOpened.AddListener(OnFlushingCompleted);
    }

    // Lock the player controls during the mini-game
    private void LockPlayerControls()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);
    }

    // Method that gets called when the lockpicking mini-game is completed successfully
    private void OnFlushingCompleted()
    {
        isUnlocked = true;

        SpawnLoot();

        // Change the safe's color to indicate it's unlocked
        GetComponent<Renderer>().material.color = Color.white;

        // Set the safe to be un-interactable
        gameObject.tag = "Untagged"; // Safe is no longer interactable
    }

    private void SpawnLoot()
    {
        // Spawn loot depending on difficulty level (example: gold, silver, etc.)?
        int spawnAmount = 0;

        if (difficulty <= 4)
            spawnAmount = 1;
        else if (difficulty <= 7)
            spawnAmount = 2;
        else
            spawnAmount = 3;

        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(loot[Random.Range(0, loot.Count)], spawnPos.position, transform.rotation);
        }

    }

    private void ResetToilet()
    {
        isUnlocked = false;
        isFlushingOpen = false;
        GetComponent<Renderer>().material.color = Color.white;  // Or the original locked color
        gameObject.tag = "Selectable";  // Re-enable interaction
    }
}
