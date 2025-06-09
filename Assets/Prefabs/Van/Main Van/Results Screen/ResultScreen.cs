using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Results screen that shows when you leave a level
 * Maybe show a different one if you lose/have less time
 * 
 * Called by Van and given reference to Van's inventory
 */

public class ResultScreen : MonoBehaviour
{
    public Dictionary<string, (int, LootInfo)> inventoryRef; // reference to any inventory (should be van but possible for player's)

    [SerializeField] float itemDelay = 0.25f;

    [Header("Dependencies")]
    [SerializeField] GameObject resultElement; // prefab that shows item stolen
    [SerializeField] Transform resultGridTransform;
    [SerializeField] TMP_Text totalStolenText;
    [SerializeField] GameObject continueButton;

    [SerializeField] Scrollbar scrollbar;
    bool scrollClicked = false;

    [SerializeField] string nextScene = "Shop";
    public void Begin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        continueButton.SetActive(false);
        StartCoroutine(CalculateTotalValue());
    }

    IEnumerator CalculateTotalValue()
    {
        yield return new WaitForSeconds(0.75f);
        int total = 0;
        foreach (var loot in inventoryRef)
        {
            int amount = loot.Value.Item1;
            LootInfo lootInfo = loot.Value.Item2;

            GameObject result = Instantiate(resultElement, resultGridTransform);
            result.GetComponent<ResultElement>().Initialize(lootInfo.sprite, lootInfo.name, amount, lootInfo.value);

            total += lootInfo.value * amount;
            totalStolenText.text = "Total Stolen: " + total;

            if (!scrollClicked)
                scrollbar.value = 0;
            yield return new WaitForSeconds(itemDelay);
        }

        GameManager.Instance.AddMoney(total);
        continueButton.SetActive(true);
    }

    public void ScrollBarClicked()
    {
        scrollClicked = true;
    }

    public void GoToShop() // used by Continue button to go to shop scene
    {
        SceneManager.LoadScene(nextScene);
    }

}
