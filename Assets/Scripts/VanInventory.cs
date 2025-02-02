using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanInventory : MonoBehaviour
{
    public static VanInventory Instance; // Singleton to access from anywhere
    public List<LootInfo> stolenItems = new List<LootInfo>(); // List of stolen items

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

    public void AddToVan(List<LootInfo> items)
    {
        stolenItems.AddRange(items);
    }

    public void ClearVan()
    {
        stolenItems.Clear();
    }
}
