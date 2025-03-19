using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class UpgradeManager : MonoBehaviour
{
    // CREATING A SINGLETON
    public static UpgradeManager Instance;

    [SerializeField]
    private float moveSpeedUpgradeIncrement = 0.5f;

    [SerializeField]
    private int maxWeightUpgradeIncrement = 3;

    [SerializeField]
    bool hasFlashlight = false;

    [SerializeField]
    private List<Items> items;

    public List<Items> loadedItems = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        loadedItems = LoadItems();
    }

    void OnDestroy()
    {
        SaveItems(loadedItems);
    }

    public void upgradeSpeed()
    {
        PlayerManager.Instance.increaseMoveSpeed(moveSpeedUpgradeIncrement);
    }

    public void upgradeMaxWeight()
    {
        PlayerManager.Instance.increaseMaxWeight(maxWeightUpgradeIncrement);
    }

    public void SetFlashlight(bool set)
    {
        hasFlashlight = set;
    }

    public bool checkFlashlight()
    {
        return hasFlashlight;
    }

    public void UpgradeScrewdriver(float increase)
    {
        PlayerManager.Instance.IncreaseBoxOpening(increase);
    }

    public void SaveItems(List<Items> items)
    {
        DataSystem.SaveItems(items);
    }

    public List<Items> LoadItems()
    {
        List<ItemData> loadedDatas = DataSystem.LoadItems();
        if (loadedDatas != null)
        {
            foreach (ItemData item in loadedDatas)
            {
                Items newItem = ScriptableObject.CreateInstance<Items>();
                newItem.Initialize(item); // Assuming you have an Initialize method to set item data
                loadedItems.Add(newItem);
            }

            foreach (Items defaultItem in items)
            {
                bool itemExists = false;
                foreach (Items loadedItem in loadedItems)
                {
                    if (loadedItem.itemName == defaultItem.itemName)
                    {
                        itemExists = true;
                        break;
                    }
                }
                if (!itemExists)
                {
                    loadedItems.Add(Instantiate(defaultItem));
                }
            }
        }
        else
        {
            loadedItems = items;    // Default list of items
            PlayerManager.Instance.ResetDefault();
        }

        return loadedItems;
    }

    public void ResetData()
    {
        File.Delete(Application.persistentDataPath + "/items.dat");
        loadedItems = LoadItems();
        GameManager.Instance.playerMoney = 0;
        hasFlashlight = false;
    }
}
