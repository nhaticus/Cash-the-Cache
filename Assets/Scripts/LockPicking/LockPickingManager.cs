using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockPickingManager : MonoBehaviour
{
    public static LockPickingManager Instance;

    public int maxAttempts = 3;
    public int currentAttempts;

    public GameObject easyPanel;
    public GameObject mediumPanel;
    public GameObject hardPanel;
    public GameObject lockpickingUI;

    private LockPicking lockPickingScript;
    private LockPickTrigger currentSafe;
    private Dictionary<string, List<int>> savedOrders = new Dictionary<string, List<int>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        currentAttempts = maxAttempts;
    }

    public void SetDifficulty(string difficulty, LockPickTrigger safe)
    {
        if (lockPickingScript == null)
        {
            lockPickingScript = FindObjectOfType<LockPicking>();

            if (lockPickingScript == null)
            {
                Debug.LogError("LockPicking script is not found! Make sure its attached to the UI Canvas.");
                return;
            }
        }

        currentSafe = safe;

        // Disable all panels
        easyPanel.SetActive(false);
        mediumPanel.SetActive(false);
        hardPanel.SetActive(false);

        // Enable the selected panel and update the lockpicking script
        if (difficulty == "Easy")
        {
            easyPanel.SetActive(true);
            lockPickingScript.SetPins(easyPanel, safe.safeID);
        }
        else if (difficulty == "Medium")
        {
            mediumPanel.SetActive(true);
            lockPickingScript.SetPins(mediumPanel, safe.safeID);
        }
        else if (difficulty == "Hard")
        {
            hardPanel.SetActive(true);
            lockPickingScript.SetPins(hardPanel, safe.safeID);
        }

        Debug.Log("Difficulty set to: " + difficulty);
        Debug.Log("New Safe Assigned: " + safe.gameObject.name);
    }
    private void DisableAllPanels()
    {
        easyPanel.SetActive(false);
        mediumPanel.SetActive(false);
        hardPanel.SetActive(false);
    }

    public void LockPickSuccess()
    {
        if (currentSafe != null)
        {
            currentSafe.MarkSafeUnlocked();
            currentSafe = null;
        }
        CloseLockpicking();
    }

    public void CloseLockpicking()
    {
        DisableAllPanels();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public bool ReduceAttempt()
    {
        currentAttempts--;
        Debug.Log("Attempts Left: " + currentAttempts);

        if (currentAttempts <= 0)
        {
            Debug.Log("Out of attempts! No more lockpicking allowed.");
            return false; // No attempts left
        }
        return true; // Can still attempt
    }

    public void ResetAllAttempts()
    {
        Debug.Log("Resetting all attempts to max!");
        currentAttempts = maxAttempts;
    }

    // Saves the correct order for a specific safe
    public void SaveOrder(string safeID, List<int> order)
    {
        if (!savedOrders.ContainsKey(safeID))
        {
            savedOrders[safeID] = new List<int>(order);
            Debug.Log($"Saved order for {safeID}: " + string.Join(", ", order));
        }
    }

    // Retrieves the saved order for a safe
    public List<int> GetOrder(string safeID)
    {
        if (savedOrders.ContainsKey(safeID))
        {
            return savedOrders[safeID];
        }
        return null;
    }

}
