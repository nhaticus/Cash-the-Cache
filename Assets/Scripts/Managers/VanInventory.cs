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

    public void AddToVan(Dictionary<string, (int, LootInfo)> playerItems)
    {
        foreach(KeyValuePair<string, (int, LootInfo)> item in playerItems)
        {
            if (stolenItems.ContainsKey(item.Key))
            {
                // stolen items = stolen items + player items
                stolenItems[item.Key] = (stolenItems[item.Key].Item1 + item.Value.Item1, item.Value.Item2);
            }
            else
            {
                stolenItems.Add(item.Key, (1, item.Value.Item2));
            }
        }
        
    }

    public void ClearVan()
    {
        stolenItems.Clear();
    }
}
