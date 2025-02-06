using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanTrigger : MonoBehaviour
{
    private bool playerInRange = false; // Track if player is in van area
    private PlayerInteract playerInventory;
    [SerializeField] GameObject canvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInteract>();

            canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;

            canvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ConvertItemsToMoney();
        }
    }

    void ConvertItemsToMoney()
    {
        if (playerInventory == null) return;

        if (VanInventory.Instance != null)
        {
            VanInventory.Instance.AddToVan(playerInventory.inventory);
        }
        else
        {
            Debug.LogError("VanInventory Instance is NULL! Make sure VanInventory exists in the scene.");
            return;
        }

        int totalMoney = 0;
        foreach (KeyValuePair<string, (int, LootInfo)> item in playerInventory.inventory)
        {
            //totalMoney += item.Key.value;
            totalMoney += item.Value.Item2.value * item.Value.Item1; // Brendan: not sure if this actually works because haven't looked at this script
        }

        if (totalMoney > 0) // Only convert if there's something to convert
        {
            GameManager.Instance.AddMoney(totalMoney);
            Debug.Log("Converted stolen items to $" + totalMoney);

            playerInventory.inventory.Clear(); // Clear inventory
            playerInventory.weight = 0; // Reset weight

            Debug.Log("Total Money: " + GameManager.Instance.playerMoney);

            //Update weight UI after removing all items
            WeightUI weightUI = FindObjectOfType<WeightUI>();
            if (weightUI != null)
            {
                weightUI.UpdateWeightDisplay();
            }

            //Log stolen items in the console after conversion
            if (VanInventory.Instance != null)
            {
                Debug.Log("Stolen Items:");
                foreach (KeyValuePair<string, (int, LootInfo)> item in VanInventory.Instance.stolenItems)
                {
                    Debug.Log(item.Key + " - $" + item.Value.Item2.value); // Brendan: also not sure if this needed the amount owned
                }
            }
            else
            {
                Debug.Log("Van Inventory is empty.");
            }
        }
        else
        {
            Debug.Log("No items to convert.");
        }
    }
}
