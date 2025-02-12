using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VanTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    private PlayerInteract playerInventory;
    [SerializeField] GameObject depositText;

    private void Awake()
    {
        depositText.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInteract>();

            if (depositText != null) depositText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;

            if (depositText != null) depositText.SetActive(false);
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