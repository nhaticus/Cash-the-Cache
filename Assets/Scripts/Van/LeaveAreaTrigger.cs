using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaveAreaTrigger : MonoBehaviour
{
    private bool playerInLeaveArea = false;
    [SerializeField] private GameObject leaveText; // "Press E to leave" text
    [SerializeField] GameObject resultScreen;

    private void Awake()
    {
        leaveText.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = true;
            if (leaveText != null)
            {
                leaveText.SetActive(true); // Show "Press E to leave"
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = false;
            if (leaveText != null) leaveText.SetActive(false);
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

            ShowSummary();
        }
    }

    void ShowSummary()
    {
        //Time.timeScale = 0;
        resultScreen.SetActive(true);
        resultScreen.GetComponent<ResultScreen>().inventoryRef = VanInventory.Instance.stolenItems;
        resultScreen.GetComponent<ResultScreen>().Begin();
        /*
        Debug.Log("Game World Frozen.");
        showingSummary = true;

        if (leaveText != null) leaveText.SetActive(false);
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
        */
    }
}