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

        int totalMoney = 0;

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

            // Calculate earnings
            totalMoney += item.Value.Item2.value * item.Value.Item1;
        }

        // Add money to player
        if (totalMoney > 0)
        {
            GameManager.Instance.AddMoney(totalMoney);
            Debug.Log("Converted stolen items to $" + totalMoney);
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


}
