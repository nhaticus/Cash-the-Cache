using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] Transform resultGridTransform;
    [SerializeField] TMP_Text totalStolenText;
    [SerializeField] string shopSceneName = "Shop Scene";
    public void Begin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(CalculateTotalValue());
    }

    IEnumerator CalculateTotalValue()
    {
        int total = 0;
        foreach (var loot in inventoryRef)
        {
            int amount = loot.Value.Item1;
            LootInfo lootInfo = loot.Value.Item2;

            GameObject result = Instantiate(resultElement, resultGridTransform);
            result.GetComponent<ResultElement>().Initialize(lootInfo.sprite, lootInfo.name, amount, lootInfo.value);

            total += lootInfo.value * amount;
            yield return new WaitForSeconds(0.4f);
        }
        totalStolenText.text = "Total Stolen: " + total;
    }

    public void GoToShop() // used by Continue button to go to shop scene
    {
        SceneManager.LoadScene(shopSceneName);
    }

}
