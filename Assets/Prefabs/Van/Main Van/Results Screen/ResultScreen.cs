using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
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
    public Dictionary<LocalizedString, (int, LootInfo)> inventoryRef; // reference to any inventory (should be van but possible for player's)

    [Header("Scrolling")]
    [SerializeField] float startDelay = 0.5f;
    [SerializeField] float itemDelay = 0.25f;
    [SerializeField] Scrollbar scrollbar;

    [Header("Dependencies")]
    [SerializeField] GameObject resultElement; // prefab that shows item stolen
    [SerializeField] Transform resultGridTransform;
    [SerializeField] LocalizeStringEvent totalStolenText;
    [SerializeField] GameObject continueButton;

    [Header("Game End")]
    [SerializeField] GameObject gameEndCanvas;
    [SerializeField] Transform canvasLocation; // where to spawn gameEndCanvas

    
    bool scrollClicked = false;

    [SerializeField] string nextScene = "Shop";

    //testing
    private void Start()
    {
        Instantiate(gameEndCanvas, canvasLocation);
    }

    public void Begin()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        continueButton.SetActive(false);

        GetTotalValue();
        StartCoroutine(CalculateTotalValue());
    }

    bool canCreateNextElement = true;
    IEnumerator CalculateTotalValue()
    {
        yield return new WaitForSeconds(startDelay);
        int total = 0;
        foreach (var loot in inventoryRef)
        {
            int amount = loot.Value.Item1;
            LootInfo lootInfo = loot.Value.Item2;
            canCreateNextElement = false;

            total += CreateElement(lootInfo, amount);

            yield return new WaitUntil(() => canCreateNextElement);

            totalStolenText.StringReference["money"] = new StringVariable { Value = total.ToString() };
            totalStolenText.RefreshString();

            if (!scrollClicked)
                scrollbar.value = 0;
            yield return new WaitForSeconds(itemDelay);
        }

        continueButton.SetActive(true);
    }

    // give money to player before showing items stolen
    void GetTotalValue()
    {
        int total = 0;
        foreach (var loot in inventoryRef)
        {
            int amount = loot.Value.Item1;
            int value = loot.Value.Item2.value;
            total += value * amount;
        }

        GameManager.Instance.AddMoney(total);
    }

    int CreateElement(LootInfo lootInfo, int amount)
    {
        GameObject result = Instantiate(resultElement, resultGridTransform);
        ResultElement RE = result.GetComponent<ResultElement>();
        RE.OnTranslationFinished += HandleElementFinished;

        RE.Initialize(lootInfo.sprite, lootInfo.translatedName, amount, lootInfo.value);
        int total = lootInfo.value * amount;

        return total;
    }

    void HandleElementFinished()
    {
        canCreateNextElement = true;
    }

    public void ScrollBarClicked()
    {
        scrollClicked = true;
    }

    public void OnContinue()
    {
        int currentDays = DataSystem.Data.gameState.currentReplay;
        int daysTotal = (int) DataSystem.GetItem("DaysTotal").statValue;

        int money = 0, moneyToEnd = 0;
        if (GameManager.Instance)
            money = GameManager.Instance.playerMoney;
        moneyToEnd = (int) DataSystem.GetItem("MoneyToEnd").statValue;

        if (daysTotal - currentDays <= 0 || money >= moneyToEnd)
        {
            GameObject endCanvas = Instantiate(gameEndCanvas, canvasLocation);
            endCanvas.GetComponent<GameEndCanvas>().Initialize();
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }

    }

}
