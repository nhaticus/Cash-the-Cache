// This script manages the player's ability to deposit items into the van when they enter the trigger area and hold 'E'.
// The items are deposited one by one over time with progress displayed on the UI.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VanTrigger : MonoBehaviour
{
    [SerializeField] GameObject vanCanvas;
    [SerializeField] GameObject vanText;

    [SerializeField] SingleAudio singleAudio;

    Coroutine depositCoroutine;

    [Header("Deposit Timing")]
    //[SerializeField] float baseLoadingTime = 1.0f; // Time before first item is deposited
    [SerializeField] float extraTimePerItem = 0.5f; // Time per item
    bool playerInRange = false;
    PlayerInteract playerInventory;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInteract>();

            vanCanvas.SetActive(true);
            vanText.SetActive(true);
            vanText.GetComponent<TMP_Text>().text = "Hold E to Deposit";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            vanText.SetActive(false);
            vanCanvas.SetActive(false);

            // If depositing, stop immediately
            if (depositCoroutine != null)
            {
                StopCoroutine(depositCoroutine);
                depositCoroutine = null;
            }
        }
    }

    private void Update()
    {
        if (!playerInRange || playerInventory == null) return;

        // Player holds E to deposit
        if (Input.GetKeyDown(KeyCode.E))
        {
            // If we're not already depositing, start the coroutine
            if (depositCoroutine == null)
            {
                // If no items, show no items message
                if (GetTotalItemCount() == 0)
                {
                    vanText.GetComponent<TMP_Text>().text = "No items to deposit!";
                }
                else
                {
                    depositCoroutine = StartCoroutine(DepositItemsOverTime());
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            // If player releases E, stop partial deposit
            if (depositCoroutine != null)
            {
                StopCoroutine(depositCoroutine);
                depositCoroutine = null;
                vanText.GetComponent<TMP_Text>().text = "Deposit canceled. Some items may have been deposited.";
            }
        }
    }

    // ------------------------------------------------------
    // Coroutine: Deposit items one-by-one over time
    // ------------------------------------------------------
    private IEnumerator DepositItemsOverTime()
    {
        // Flatten or count your items first:
        List<(string, LootInfo)> allItems = FlattenInventory(playerInventory);
        int totalItems = allItems.Count;
        if (totalItems == 0)
        {
            vanText.GetComponent<TMP_Text>().text = "No items to deposit!";
            yield break;
        }

        // Calculate total time = (items * extraTimePerItem)
        float totalTime = totalItems * extraTimePerItem;
        float depositTimer = 0f;

        // Deposit items one by one
        for (int i = 0; i < totalItems; i++)
        {
            singleAudio.PlaySFX("deposit_sound");
            float itemTimer = 0f;
            while (itemTimer < extraTimePerItem)
            {
                // If player releases E, stop immediately
                if (!Input.GetKey(KeyCode.E))
                    yield break; // canceled

                itemTimer += Time.deltaTime;
                depositTimer += Time.deltaTime;

                // Convert depositTimer to a single 0-100% for the entire operation
                float progressFraction = depositTimer / totalTime;
                int percent = Mathf.Clamp((int)(progressFraction * 100f), 0, 100);
                vanText.GetComponent<TMP_Text>().text = $"Depositing... {percent}%";
                yield return null;
            }

            // After waiting 1 second (extraTimePerItem), deposit this item
            var (itemName, info) = allItems[i];
            RemoveOneItemFromPlayer(playerInventory, itemName, info);
        }

        if (TaskManager.Instance != null)
            TaskManager.Instance.task2Complete();

        // Done depositing everything
        depositCoroutine = null;
        vanText.GetComponent<TMP_Text>().text = "All items deposited!";
    }

    // ------------------------------------------------------
    // Inventory Helpers
    // ------------------------------------------------------

    private int GetTotalItemCount()
    {
        int total = 0;
        foreach (var kvp in playerInventory.inventory)
        {
            total += kvp.Value.Item1; // (int quantity, LootInfo info)
        }
        return total;
    }

    // Convert dictionary { itemName -> (count, lootInfo) } into a list of items
    private List<(string, LootInfo)> FlattenInventory(PlayerInteract pInv)
    {
        var result = new List<(string, LootInfo)>();
        foreach (var kvp in pInv.inventory)
        {
            string itemName = kvp.Key;
            int quantity = kvp.Value.Item1;
            LootInfo info = kvp.Value.Item2;
            for (int i = 0; i < quantity; i++)
            {
                result.Add((itemName, info));
            }
        }
        return result;
    }

    // Remove one item from player's dictionary and add to VanInventory
    private void RemoveOneItemFromPlayer(PlayerInteract pInv, string itemName, LootInfo info)
    {
        if (!pInv.inventory.ContainsKey(itemName)) return;

        var (count, loot) = pInv.inventory[itemName];
        count--;
        // Remove from player's dictionary
        if (count <= 0)
        {
            pInv.inventory.Remove(itemName);
        }
        else
        {
            pInv.inventory[itemName] = (count, loot);
        }

        // Adjust player's weight
        int newWeight = PlayerManager.Instance.getWeight() - loot.weight;
        PlayerManager.Instance.setWeight(Mathf.Max(0, newWeight));
        PlayerManager.Instance.WeightChangeSpeed();

        // Actually deposit into van
        if (VanInventory.Instance != null)
        {
            VanInventory.Instance.DepositSingleItem(itemName, info);
        }
    }
}