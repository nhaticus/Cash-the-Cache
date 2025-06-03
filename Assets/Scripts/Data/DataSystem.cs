using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class DataSystem
{
    private static string ItemFilePath => Path.Combine(Application.persistentDataPath, "items.json");
    private static ItemData upgradeData;

    private static string SettingFilePath => Path.Combine(Application.persistentDataPath, "settings.json");
    private static GameSettingsData settingData;

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
        File.WriteAllText(ItemFilePath, json);
        Debug.Log("Data file path: " + ItemFilePath);
    }

    public static void LoadItems()
    {
        if (File.Exists(ItemFilePath))
        {
            string json = File.ReadAllText(ItemFilePath);
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

    /// <summary> Retrieves an exiting item by name, or creates a new one if it doesn't exist. </summary>
    /// <param name="name"> Name of item to retrieve or create EX: "Backpack". </param>
    /// <param name="defaultStatValue"> Stat value to assign to item EX: 5.0f means each "Backpack" upgrade will be +5 weight. </param>
    /// <returns> Item with specified name, or new item if it doesn't already exist. </returns>
    public static Item GetOrCreateItem(string name, float defaultStatValue = 0f, bool forceUpdateStatValue = false)
    {
        var item = GetItem(name);
        if (item == null)
        {
            item = new Item { itemName = name, level = 0, statValue = defaultStatValue };
            Data.items.Add(item);
        }
        else if (item.statValue != defaultStatValue && forceUpdateStatValue)
        {
            item.statValue = defaultStatValue;
            // Debug.Log($"Updated statValue of '{name}' to default: {defaultStatValue}");
        }
        return item;
    }

    public static void ResetItems()
    {
        foreach (var item in Data.items)
        {
            item.level = 0;
        }
        SaveItems();
    }

    public static GameSettingsData SettingsData
    {
        get
        {
            if (settingData == null)
                LoadSettings();
            return settingData;
        }
    }
    public static void SaveSettings()
    {
        string json = JsonUtility.ToJson(settingData, true);
        File.WriteAllText(SettingFilePath, json);
    }

    public static void LoadSettings()
    {
        if (File.Exists(SettingFilePath))
        {
            string json = File.ReadAllText(SettingFilePath);
            settingData = JsonUtility.FromJson<GameSettingsData>(json);
        }
        else
        {
            ResetSettings();
        }
    }

    public static void ResetSettings()
    {
        if (File.Exists(SettingFilePath))
        {
            File.Delete(SettingFilePath);
        }
        settingData = new GameSettingsData();
        SaveSettings();
    }
}
