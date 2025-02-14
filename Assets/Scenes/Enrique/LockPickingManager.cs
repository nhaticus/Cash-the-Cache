using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockPickingManager : MonoBehaviour
{
    public GameObject easyPanel;
    public GameObject mediumPanel;
    public GameObject hardPanel;
    public GameObject lockpickingUI;

    private LockPicking lockPickingScript;

    
    void Start()
    {
   
    }
    public void SetDifficulty(string difficulty)
    {
            if (lockPickingScript == null)
            {
                lockPickingScript = FindObjectOfType<LockPicking>();

                if (lockPickingScript == null)
                {
                    Debug.LogError("LockPicking script is STILL NOT found! Make sure it is attached to the UI Canvas.");
                    return;
                }
            }
            // Disable all panels
            easyPanel.SetActive(false);
        mediumPanel.SetActive(false);
        hardPanel.SetActive(false);

        // Enable the selected panel and update the lockpicking script
        if (difficulty == "Easy")
        {
            easyPanel.SetActive(true);
            lockPickingScript.SetPins(easyPanel);
        }
        else if (difficulty == "Medium")
        {
            mediumPanel.SetActive(true);
            lockPickingScript.SetPins(mediumPanel);
        }
        else if (difficulty == "Hard")
        {
            hardPanel.SetActive(true);
            lockPickingScript.SetPins(hardPanel);
        }

        Debug.Log("Difficulty set to: " + difficulty);
    }

    public void CloseLockpicking()
    {
        lockpickingUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
