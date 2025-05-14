// Safe handles the interactions with the safe object, including the lockpicking mini-game and loot spawning.
// Contains Logic for:
//  - Opening the lock picking mingiame and preventing multiple interactions
//  - Handling the lockpicking success and failure
//  - Spawning loot based on difficulty
//  - Locking safe if failed
using UnityEngine;

public class Safe : MonoBehaviour, InteractEvent
{
    [Header("Safe Settings")]
    public bool isUnlocked = false;  // Tracks whether the safe is unlocked or not
    public Animator safeAnimator;    // Animator for the safe's animation
    public int maxAttempts = 3;      // Max number of attempts for the lockpicking game

    [Header("Diffculty = number of pins")]
    [SerializeField] int difficulty = 1; // Difficulty level = number of pins
    public bool isLockpickingOpen = false;  // Whether the lockpicking mini-game is open or not

    [Header("Canvas Settings")]
    [SerializeField] GameObject lockCanvasPrefab; // Prefab for the lockpicking UI canvas
    private GameObject currentCanvas; // Reference to the currently active canvas

    [Header("Loot Settings")]
    [SerializeField] GameObject gold;
    [SerializeField] Transform spawnPos;

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

        // Check if the canvas already exists and is active
        if (currentCanvas == null)
        {
            InitializeLockPickingCanvas();
        }
        else
        {
            // If the canvas exists, just enable it and retain the combo
            currentCanvas.SetActive(true);

            LockPickingCanvas canvasScript = currentCanvas.GetComponent<LockPickingCanvas>();

            // Restart the flashing effect but don't reset the order of the pins
            StartCoroutine(canvasScript.AssignPinOrderEffect()); // Re-start flashing
        }

        LockPlayerControls();
    }

    private void InitializeLockPickingCanvas()
    {
        currentCanvas = Instantiate(lockCanvasPrefab, transform);
        currentCanvas.SetActive(true);
        LockPickingCanvas canvasScript = currentCanvas.GetComponent<LockPickingCanvas>();

        // Set the difficulty for the lockpicking mini-game and add listeners for success and failure
        canvasScript.difficulty = difficulty;
        canvasScript.SetMaxAttempts(maxAttempts);
        canvasScript.LockOpened.AddListener(OnLockpickingCompleted);
        canvasScript.LockFailed.AddListener(OnLockpickingFailed);
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
    private void OnLockpickingFailed() 
    {
        gameObject.tag = "Untagged"; // Safe is no longer interactable
    }

    // Method to handle loot spawning when the safe is unlocked
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
            Instantiate(gold, spawnPos.position, transform.rotation);
        }
    }

    private void ResetSafe()
    {
        isUnlocked = false;
        isLockpickingOpen = false;
        GetComponent<Renderer>().material.color = Color.gray;
        gameObject.tag = "Selectable";  // Re-enable interaction
    }

}
