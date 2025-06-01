// This script manages the player's ability to deposit items into the van when they enter the trigger area and hold 'E'.
// The items are deposited one by one over time with progress displayed on the UI.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class VanTrigger : MonoBehaviour
{
    bool playerInRange = false;
    PlayerInteract playerInventory;
    [SerializeField] GameObject vanText;

    [SerializeField] SingleAudio singleAudio;

    Coroutine depositCoroutine;

    [Header("Deposit Timing")]
    //[SerializeField] float baseLoadingTime = 1.0f; // Time before first item is deposited
    [SerializeField] float extraTimePerItem = 0.5f; // Time per item

    VanInventory vanInventory;

    [Header("Localization")]
    [SerializeField] LocalizedString holdToDepositString;
    [SerializeField] LocalizedString noItemsToDepositString;
    [SerializeField] LocalizedString depositingProgressString;
    [SerializeField] LocalizedString depositCanceledString;
    [SerializeField] LocalizedString allItemsDepositedString;

    private void Awake()
    {
        vanText.SetActive(false);
        vanInventory = transform.parent.GetComponent<VanInventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInteract>();

            vanText.SetActive(true);
            //vanText.GetComponent<TMP_Text>().text = "Hold E to Deposit";
            holdToDepositString.StringChanged += UpdateVanText;
            holdToDepositString.RefreshString();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
            vanText.SetActive(false);

            // If depositing, stop immediately
            if (depositCoroutine != null)
            {
                StopCoroutine(depositCoroutine);
                depositCoroutine = null;
            }
            holdToDepositString.StringChanged -= UpdateVanText;
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
                    //vanText.GetComponent<TMP_Text>().text = "No items to deposit!";
                    noItemsToDepositString.StringChanged += UpdateVanText;
                    noItemsToDepositString.RefreshString();
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
                
                depositCanceledString.StringChanged += UpdateVanText;
                depositCanceledString.RefreshString();                
                //vanText.GetComponent<TMP_Text>().text = "Deposit canceled. Some items may have been deposited.";
            }
        }
    }

    
    private void UpdateVanText(string localizedText)
    {
        vanText.GetComponent<TMP_Text>().text = localizedText;
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
           // vanText.GetComponent<TMP_Text>().text = "No items to deposit!";
            noItemsToDepositString.StringChanged += UpdateVanText;
            noItemsToDepositString.RefreshString();
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
                //vanText.GetComponent<TMP_Text>().text = $"Depositing... {percent}%";
                depositingProgressString.Arguments = new object[] { percent };
                depositingProgressString.StringChanged += UpdateVanText;
                depositingProgressString.RefreshString();

                yield return null;
            }

            // After waiting 1 second (extraTimePerItem), deposit this item
            var (itemName, info) = allItems[i];
            RemoveOneItemFromPlayer(playerInventory, itemName, info);
        }

        // Done depositing everything
        depositCoroutine = null;
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.task2Complete();
        }
        //vanText.GetComponent<TMP_Text>().text = "All items deposited!";
        allItemsDepositedString.StringChanged += UpdateVanText;
        allItemsDepositedString.RefreshString();

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

        // Deposit into van
        if (vanInventory != null)
        {
            vanInventory.DepositSingleItem(itemName, info);
        }
    }
}