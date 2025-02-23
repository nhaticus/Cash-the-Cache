using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Collider for player to deposit items
 */

public class VanTrigger : MonoBehaviour
{
    bool playerInRange = false;
    PlayerInteract playerInventory;
    [SerializeField] GameObject vanText;

    private void Awake()
    {
        vanText.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInteract>();

            vanText.SetActive(true);
            vanText.GetComponent<TMP_Text>().text = "Press E to Deposit";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;

            vanText.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            DepositItems();
        }
    }

    private void DepositItems()
    {
        if (VanInventory.Instance != null && playerInventory != null)
        {
            VanInventory.Instance.TransferItemsFromPlayer(playerInventory);
        }
        else
        {
            Debug.LogError("VanInventory or PlayerInventory is NULL!");
        }
    }
}