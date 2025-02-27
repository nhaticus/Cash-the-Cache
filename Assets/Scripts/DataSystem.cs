using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataSystem
{
    public static void SaveItems(Items[] items)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/items.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, items);
        stream.Close();
    }

    public static Items[] LoadItems()
    {
        string path = Application.persistentDataPath + "/items.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Items[] items = formatter.Deserialize(stream) as Items[];
            stream.Close();
            return items;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
