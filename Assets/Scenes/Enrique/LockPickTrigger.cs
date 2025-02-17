using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickTrigger : MonoBehaviour
{
    public string difficulty = "Easy"; // Default difficulty is 'Easy
    private bool isNearSafe = false;
    private bool isUnlocked = false;
    public LockPickingManager lockPickingManager;
    public Animator safeAnimator;
    public string safeID;

    private void Start()
    {
        if (string.IsNullOrEmpty(safeID))
        {
            safeID = gameObject.name; // Assign the GameObject name if no ID is given
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (isNearSafe && !isUnlocked && Input.GetKeyDown(KeyCode.E))
        {
            if (lockPickingManager.currentAttempts > 0) // Prevent interaction if no attempts left
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
        if (other.CompareTag("Player")) // Check if the player entered the safe area
        {
            Debug.Log("Player is in safe area");
            isNearSafe = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player left the safe area
        {
            Debug.Log("Player left safe area");
            isNearSafe = false;
        }
    }

    void OpenLockpicking()
    {
        PlayerManager.Instance.lockRotation();
        SetCursorState(true);

        lockPickingManager.lockpickingUI.SetActive(true);
        lockPickingManager.SetDifficulty(difficulty, this);

        
    }
    public void MarkSafeUnlocked()
    {
        //if (isUnlocked) return;
        isUnlocked = true;
        Debug.Log("Safe Unlocked: " + gameObject.name);
        safeAnimator.SetTrigger("OpenSafe");
    }
    private void SetCursorState(bool visible)
    {
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor and make it visible
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor and hide it
        }
        Cursor.visible = visible;
    }
}
