using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * Collider for player to leave level
 */

public class LeaveAreaTrigger : MonoBehaviour
{
    bool playerInLeaveArea = false;
    [SerializeField] GameObject vanText;
    [SerializeField] GameObject resultScreen;

    private void Awake()
    {
        vanText.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = true;
            vanText.SetActive(true);
            vanText.GetComponent<TMP_Text>().text = "Press E to leave";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = false;
            vanText.SetActive(false);
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
        resultScreen.SetActive(true);
        resultScreen.GetComponent<ResultScreen>().inventoryRef = VanInventory.Instance.stolenItems;
        resultScreen.GetComponent<ResultScreen>().Begin();
    }
}