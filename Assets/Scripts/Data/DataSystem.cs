using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public static class DataSystem
{
    public static void SaveItems(List<Items> items)
    {
        // Debug.Log("Saving items");
        BinaryFormatter formatter = new();
        string path = Application.persistentDataPath + "/items.dat";
        FileStream fs = new(path, FileMode.Create);

        List<ItemData> itemDatas = new();
        foreach (Items item in items)
        {
            itemDatas.Add(new ItemData(item));
        }

        formatter.Serialize(fs, itemDatas);
        fs.Close();
    }

    public static List<ItemData> LoadItems()
    {
        string path = Application.persistentDataPath + "/items.dat";
        if (File.Exists(path))
        {
            // Debug.Log("Loading items from " + path);
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);

            List<ItemData> itemDatas = formatter.Deserialize(stream) as List<ItemData>;
            stream.Close();
            return itemDatas;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }
}
