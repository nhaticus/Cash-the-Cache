using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject[] obj;  // Objects to spawn upon success (loot, etc.)
    public bool isUnlocked = false;  // Tracks whether the safe is unlocked or not
    public Animator safeAnimator;    // Animator for the safe's animation
    public int maxAttempts = 3;      // Max number of attempts for the lockpicking game

    [SerializeField] int difficulty = 1; // Difficulty level = number of pins
    public bool isLockpickingOpen = false;  // Whether the lockpicking mini-game is open or not

    [Header("Canvas Settings")]
    [SerializeField] GameObject lockCanvasPrefab; // Prefab for the lockpicking UI canvas

    Vector3[] originalPositions;

    // This method is called when the player interacts with the safe
    public void Interact()
    {
        if (isLockpickingOpen)
        {
            Debug.Log("Already interacting with the lockpicking minigame.");
            return;  // If lockpicking is already open, don't allow further interaction
        }

        isLockpickingOpen = true;
        
        ResetSafe();

        // Instantiate the LockPickingCanvas prefab and set difficulty
        GameObject canvas = Instantiate(lockCanvasPrefab, transform);
        LockPickingCanvas canvasScript = canvas.GetComponent<LockPickingCanvas>();

        // Set the difficulty for the lockpicking mini-game and add a listener for when the lock is opened
        canvasScript.difficulty = difficulty;
        canvasScript.LockOpened.AddListener(OnLockpickingCompleted); // Add listener for success event

        // Lock the player controls during the mini-game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);
    }

    // Method that gets called when the lockpicking mini-game is completed successfully
    private void OnLockpickingCompleted()
    {
        // Unlock the safe and play the animation
        isUnlocked = true;
        if (safeAnimator != null)
        {
            safeAnimator.SetTrigger("OpenSafe"); // Trigger safe opening animation
        }

        // Spawn loot based on difficulty
        SpawnLoot();

        // Change the safe's color to indicate it's unlocked
        GetComponent<Renderer>().material.color = Color.green;

        // Set the safe to be un-interactable
        gameObject.tag = "Untagged"; // Safe is no longer interactable
    }

    // Method to handle loot spawning when the safe is unlocked
    private void SpawnLoot()
    {
        GameObject loot = null;

        // Spawn loot depending on difficulty level (example: gold, silver, etc.)?
        if (obj.Length > 0)
        {
            loot = Instantiate(obj[0], transform.position, transform.rotation);
        }
    }

    private void ResetSafe()
    {
        isUnlocked = false;
        isLockpickingOpen = false;
        GetComponent<Renderer>().material.color = Color.gray;  // Or the original locked color
        gameObject.tag = "Selectable";  // Re-enable interaction
    }

}
