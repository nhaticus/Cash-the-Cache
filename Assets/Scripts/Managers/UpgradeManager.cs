using UnityEngine;
using System.IO;

// public class UpgradeData
// {
//     public string itemName;
//     public string description;
//     public int level;
//     public string stats;
//     public int price;

//     public UpgradeData(Items item)
//     {
//         itemName = item.itemName;
//         description = item.description;
//         level = item.level;
//         stats = item.stats;
//         price = item.price;
//     }
// }
public class UpgradeManager : MonoBehaviour
{
    // CREATING A SINGLETON
    public static UpgradeManager Instance;

    [SerializeField]
    private float moveSpeedUpgradeIncrement = 0.5f;

    [SerializeField]
    private int maxWeightUpgradeIncrement = 3;

    public Items[] items;

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
        string path = Application.persistentDataPath + "/items.dat";
        // if (!File.Exists(path))
        // {
        //     SaveItems(items);
        // }
        // LoadItems();
    }

    public void upgradeSpeed()
    {
        PlayerManager.Instance.increaseMoveSpeed(moveSpeedUpgradeIncrement);
    }

    public void upgradeMaxWeight()
    {
        PlayerManager.Instance.increaseMaxWeight(maxWeightUpgradeIncrement);
    }
    public void SaveItems(Items[] items)
    {
        DataSystem.SaveItems(items);
    }

    public void LoadItems()
    {
        Items[] data = DataSystem.LoadItems();
        if (data != null)
        {
            foreach (Items item in data)
            {
                Debug.Log(item.itemName);
            }
        }
    }
}
