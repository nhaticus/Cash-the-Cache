using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class DataSystem
{
    private static string filePath => Path.Combine(Application.persistentDataPath, "items.json");

    private static ItemData upgradeData;

    public static ItemData Data
    {
        get
        {
            if (upgradeData == null)
                LoadItems();
            return upgradeData;
        }
    }

    public static void SaveItems()
    {
        string json = JsonUtility.ToJson(upgradeData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data file path: " + filePath);
    }

    public static void LoadItems()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            upgradeData = JsonUtility.FromJson<ItemData>(json);
        }
        else
        {
            upgradeData = new ItemData();
        }
    }

    public static Item GetItem(string name)
    {
        return Data.items.Find(i => i.itemName == name);
    }

    public static Item GetOrCreateItem(string name)
    {
        var item = GetItem(name);
        if (item == null)
        {
            item = new Item { itemName = name, level = 0, statValue = 0 };
            Data.items.Add(item);
        }
        return item;
    }

    public static void ResetItems()
    {
        foreach (var item in Data.items)
        {
            item.level = 0;
        }
    }
}
