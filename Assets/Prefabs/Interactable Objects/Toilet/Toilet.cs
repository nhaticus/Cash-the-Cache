using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This uses the lock picking cs file as a template, and dictates the flushing minigame
public class Toilet : MonoBehaviour, InteractEvent
{
    [Header("Toilet Settings")]
    public bool isUnlocked = false;  // Tracks whether the toilet is flushed or not

    [Header("Diffculty = Time it takes to flush")]
    [SerializeField] int difficulty = 3; // Difficulty level = number of seconds x 1
    public bool isFlushingOpen = false;  // Whether the flushing mini-game is open or not

    [Header("Canvas Settings")]
    [SerializeField] GameObject FlushingCanvasPrefab; // Prefab for the flushing UI canvas
    private GameObject currentCanvas; // Reference to the currently active canvas

    [Header("Loot Settings")]
    [SerializeField] List<GameObject> loot;
    [SerializeField] GameObject el_Skibidi; //yeah idk man when i delete this line of code everything breaks so i left it in
    [SerializeField] Transform spawnPos;

    // This method is called when the player interacts with the safe
    public void Interact()
    {
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
        else // canvas exists, enable it and retain the combo
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

        // Spawn loot based on difficulty
        SpawnLoot();

        // Change the safe's color to indicate it's unlocked
        GetComponent<Renderer>().material.color = Color.grey;

        // Set the safe to be un-interactable
        gameObject.tag = "Untagged"; // Safe is no longer interactable
    }


    // Method to handle loot spawning when the safe is unlocked
    private void SpawnLoot()
    {
        // Spawn loot depending on difficulty level (example: gold, silver, etc.)?
        int spawnAmount = 0;

        if (difficulty <= 4)
        {
            spawnAmount = 1;
        }
        else if (difficulty <= 7)
        {
            spawnAmount = 2;
        }
        else
        {
            spawnAmount = 3;
        }
        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(loot[Random.Range(0, loot.Count)], spawnPos.position, transform.rotation);
        }
        if(Random.Range(1, 101) < 2) el_Skibidi.SetActive(true);

    }

    private void ResetToilet()
    {
        isUnlocked = false;
        isFlushingOpen = false;
        GetComponent<Renderer>().material.color = Color.white;  // Or the original locked color
        gameObject.tag = "Selectable";  // Re-enable interaction
    }
}
