using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickTrigger : MonoBehaviour
{
    public string safeID;
    private bool isNearSafe = false;
    private bool isUnlocked = false;
    public Animator safeAnimator;
    public string difficulty = "Easy";

    // Your existing fields:
    public GameObject lockpickingUI;
    public GameObject easyPanel;
    public GameObject mediumPanel;
    public GameObject hardPanel;

    public int maxAttempts = 3;
    public int currentAttempts;
    private List<int> savedOrder;

    private void Start()
    {
        if (string.IsNullOrEmpty(safeID))
        {
            safeID = gameObject.name;
        }
        currentAttempts = maxAttempts;
    }

    // Interaction code remains unchanged except for using currentAttempts directly.
    private void Update()
    {
        if (isNearSafe && !isUnlocked && Input.GetMouseButtonDown(0) && !FindObjectOfType<InventoryUI>().isInventoryOpen)
        {
            if (currentAttempts > 0)
            {
                Debug.Log("Lock Picking Started");
                OpenLockpicking();
            }
            else
            {
                Debug.Log("No attempts left! Can't pick the lock.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in safe area");
            isNearSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left safe area");
            isNearSafe = false;
        }
    }

    void OpenLockpicking()
    {
        PlayerManager.Instance.lockRotation();
        SetCursorState(true);

        if (lockpickingUI != null)
        {
            lockpickingUI.SetActive(true);
            StartLockPicking();
        }
        else
        {
            Debug.LogError("Lockpicking UI is not assigned!");
        }
    }

    // Activates the correct UI panel based on difficulty and initializes the lock-picking UI.
    void StartLockPicking()
    {
        if (easyPanel != null) easyPanel.SetActive(false);
        if (mediumPanel != null) mediumPanel.SetActive(false);
        if (hardPanel != null) hardPanel.SetActive(false);

        LockPicking lp = lockpickingUI.GetComponentInChildren<LockPicking>();
        if (lp == null)
        {
            Debug.LogError("LockPicking script not found in lockpicking UI!");
            return;
        }

        if (difficulty == "Easy")
        {
            if (easyPanel != null)
            {
                easyPanel.SetActive(true);
                lp.SetPins(easyPanel, this);
            }
        }
        else if (difficulty == "Medium")
        {
            if (mediumPanel != null)
            {
                mediumPanel.SetActive(true);
                lp.SetPins(mediumPanel, this);
            }
        }
        else if (difficulty == "Hard")
        {
            if (hardPanel != null)
            {
                hardPanel.SetActive(true);
                lp.SetPins(hardPanel, this);
            }
        }
        else
        {
            Debug.LogError("Unknown difficulty level: " + difficulty);
        }

        Debug.Log("Difficulty set to: " + difficulty);
    }

    public void MarkSafeUnlocked()
    {
        isUnlocked = true;
        Debug.Log("Safe Unlocked: " + gameObject.name);
        if (safeAnimator != null)
        {
            safeAnimator.SetTrigger("OpenSafe");
        }
        else
        {
            Debug.LogError("Safe animator not assigned!");
        }
    }

    // Returns the saved combination order.
    public List<int> GetOrder()
    {
        return savedOrder;
    }

    // Saves the combination order.
    public void SaveOrder(List<int> order)
    {
        if (savedOrder == null)
        {
            savedOrder = new List<int>(order);
            Debug.Log($"Saved order for {safeID}: " + string.Join(", ", order));
        }
    }

    // Reduces the attempt count and returns false if no attempts remain.
    public bool ReduceAttempt()
    {
        currentAttempts--;
        Debug.Log("Attempts Left: " + currentAttempts);
        if (currentAttempts <= 0)
        {
            Debug.Log("Out of attempts! No more lockpicking allowed.");
            return false;
        }
        return true;
    }

    private void SetCursorState(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
