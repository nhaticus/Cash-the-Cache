using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Results screen that shows when you leave a level
 * Maybe a different one if you lose
 * 
 * Should be called by Van and is given reference to Van's inventory
 */

public class ResultScreen : MonoBehaviour
{
    public Dictionary<string, (int, LootInfo)> inventoryRef; // reference to any inventory (should be van but possible for player's)
    [SerializeField] GameObject resultElement; // prefab that shows item stolen
    [SerializeField] TMP_Text totalStolenText;

    

    private void Start()
    {
        totalStolenText.text = "Total stolen: " + CalculateTotalValue();
    }

    int CalculateTotalValue()
    {
        int total = 0;
        foreach (var loot in inventoryRef)
        {
            int amount = loot.Value.Item1;
            LootInfo lootInfo = loot.Value.Item2;
            total += lootInfo.value * amount;
        }
        return total;
    }

    public void GoToShop() // used by Continue button to go to shop scene
    {

    }

}
