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

    // Update is called once per frame
    void Update()
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
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player entered the safe area
        {
            Debug.Log("Player is in safe area");
            isNearSafe = true;
        }
    }
    void OnTriggerExit(Collider other)
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


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

}
