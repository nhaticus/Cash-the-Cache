//Script manges the van's inventoiry, allowing stolen items to be transfered from the player inventroy
//and desposited individually. It maintains a singleton instance to access the inventory from anywhere in the game.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanInventory : MonoBehaviour
{
    public static VanInventory Instance; // Singleton to access from anywhere
    public Dictionary<string, (int, LootInfo)> stolenItems = new Dictionary<string, (int, LootInfo)>(); // Dictionary of stolen items

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TransferItemsFromPlayer(PlayerInteract playerInventory)
    {
        if (playerInventory == null || playerInventory.inventory.Count == 0)
        {
            Debug.Log("No items to transfer.");
            return;
        }

        foreach (KeyValuePair<string, (int, LootInfo)> item in playerInventory.inventory)
        {
            // Add items to van inventory
            if (stolenItems.ContainsKey(item.Key))
            {
                stolenItems[item.Key] = (stolenItems[item.Key].Item1 + item.Value.Item1, item.Value.Item2);
            }
            else
            {
                stolenItems.Add(item.Key, (item.Value.Item1, item.Value.Item2));
            }
        }

        // Clear player's inventory
        playerInventory.inventory.Clear();
        PlayerManager.Instance.setWeight(0);

        // Update UI
        WeightUI weightUI = FindObjectOfType<WeightUI>();
        if (weightUI != null)
        {
            weightUI.UpdateWeightDisplay();
        }
    }



    public void ClearVan()
    {
        stolenItems.Clear();
    }

    public void DepositSingleItem(string itemName, LootInfo info)
    {
        if (stolenItems.ContainsKey(itemName))
        {
            // Increase the existing count by 1
            stolenItems[itemName] = (stolenItems[itemName].Item1 + 1, info);
        }
        else
        {
            // Create a new entry with count = 1
            stolenItems.Add(itemName, (1, info));
        }

        // (Optional) If you want to update a UI element after each item deposit:
        WeightUI weightUI = FindObjectOfType<WeightUI>();
        if (weightUI != null)
        {
            weightUI.UpdateWeightDisplay();
        }
    }
}
