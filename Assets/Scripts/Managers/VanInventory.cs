using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanInventory : MonoBehaviour
{
    public static VanInventory Instance; // Singleton to access from anywhere
    public Dictionary<LootInfo, int> stolenItems = new Dictionary<LootInfo, int>(); // List of stolen items

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

    public void AddToVan(Dictionary<LootInfo, int> items)
    {
        foreach(KeyValuePair<LootInfo, int> info in items)
        {
            try
            {
                stolenItems.Add(info.Key, 1);
            }
            catch (ArgumentException)
            {
                stolenItems[info.Key] += 1;
            }
        }
        
    }

    public void ClearVan()
    {
        stolenItems.Clear();
    }
}
