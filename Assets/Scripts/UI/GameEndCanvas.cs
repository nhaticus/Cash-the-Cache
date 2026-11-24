using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndCanvas : MonoBehaviour
{
    [Header("Days")]
    int daysLeft;
    [SerializeField] TMP_Text daysText;

    [Header("Money")]
    int money, moneyToEnd;
    [SerializeField] TMP_Text moneyText, moneyRequireText, moneyPercentageText;

    void Start()
    {
        int currentDays = DataSystem.Data.gameState.currentReplay;
        //daysLeft = daysTotal - currentDays;
        if (GameManager.Instance)
        {
            money = GameManager.Instance.playerMoney;
            moneyToEnd = GameManager.Instance.moneyToEnd;
        }


        daysText.text = daysLeft.ToString();
        moneyText.text = "$" + money.ToString();
        moneyRequireText.text = "$" + moneyToEnd.ToString();
    }

}
