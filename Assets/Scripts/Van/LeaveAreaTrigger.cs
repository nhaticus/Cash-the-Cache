using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class LeaveAreaTrigger : MonoBehaviour
{
    private bool playerInLeaveArea = false;
    private bool showingSummary = false;
    [SerializeField] private GameObject leaveText;
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText; // Text for stolen items summary
    [SerializeField] private string shopSceneName = "ShopScene"; // Change later


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = true;

            if (leaveText != null)
            {
                leaveText.SetActive(true); // Show "Press E to leave"
            }

            Debug.Log("Press E to leave.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = false;

            if (leaveText != null)
            {
                leaveText.SetActive(false); // Hide text
            }

            Debug.Log("Left leave area.");
        }
    }

    private void Update()
    {
        if (playerInLeaveArea && Input.GetKeyDown(KeyCode.E))
        {
            if (!showingSummary)
            {
                ShowSummary();
            }
            else
            {
                LeaveLevel();
            }
        }
    }
    void ShowSummary()
    {
        //Freeze the world (but keep UI working)
        Time.timeScale = 0;
        Debug.Log("Game World Frozen.");

        showingSummary = true;

        if (leaveText != null)
        {
            leaveText.SetActive(false); // Hide "Press E to leave" text
        }

        if (summaryPanel != null)
        {
            summaryPanel.SetActive(true); // Show summary panel
        }

        // Generate the summary text
        int totalEarnings = 0;
        string summary = "Stolen Items:\n";

        foreach (KeyValuePair<LootInfo, int> item in VanInventory.Instance.stolenItems)
        {
            summary += $"{item.Key.name} - ${item.Key.value}\n";
            totalEarnings += item.Key.value;
        }

        summary += $"\nTotal Earned: ${totalEarnings}";
        summaryText.text = summary; // Set UI text

        Debug.Log("Summary displayed. Press E again to continue.");
    }

    void LeaveLevel()
    {

        Debug.Log("Leaving level... Loading shop scene.");
        // Unfreeze the world before switching scenes
        Time.timeScale = 1;
        // SceneManager.LoadScene(shopSceneName); //Loads Shop Scene
    }
}
