using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaveAreaTrigger : MonoBehaviour
{
    private bool playerInLeaveArea = false;
    private bool showingSummary = false;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText; // Text for stolen items summary
    [SerializeField] private string shopSceneName = "ShopScene"; // Change later

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = true;
            if (canvas != null)
            { 
                canvas.SetActive(true); // Show "Press E to leave"
            }
            Debug.Log("Press E to leave.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = false;
            if (canvas != null) canvas.SetActive(false);
            Debug.Log("Left leave area.");
        }
    }

    private void Update()
    {
        if (playerInLeaveArea && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInteract playerInventory = FindObjectOfType<PlayerInteract>();

            if (VanInventory.Instance != null && playerInventory != null)
            {
                VanInventory.Instance.TransferItemsFromPlayer(playerInventory);
            }
            else
            {
                Debug.LogError("VanInventory or PlayerInventory is NULL!");
            }

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
        Time.timeScale = 0;
        Debug.Log("Game World Frozen.");
        showingSummary = true;

        if (canvas != null) canvas.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(true);

        int totalEarnings = 0;
        string summary = "Stolen Items:\n";

        foreach (KeyValuePair<string, (int, LootInfo)> item in VanInventory.Instance.stolenItems)
        {
            int itemTotalValue = item.Value.Item2.value * item.Value.Item1;
            summary += $"{item.Key} x{item.Value.Item1} - ${itemTotalValue}\n";
            totalEarnings += itemTotalValue;
        }

        summary += $"\nTotal Earned: ${totalEarnings}";
        summaryText.text = summary; // Set UI text

        Debug.Log("Summary displayed. Press E again to continue.");
    }

    void LeaveLevel()
    {
        Debug.Log("Leaving level... Loading shop scene.");
        Time.timeScale = 1;
        // SceneManager.LoadScene(shopSceneName); // Uncomment this to load the next scene
    }
}