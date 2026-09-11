using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DaysInfo : MonoBehaviour
{
    [Header("Days")]
    [SerializeField] int daysLeft = 45; // when reach 0, BAD ending
    [SerializeField] int daysTotal = 45;
    [SerializeField] TMP_Text daysText;

    [Header("Money")]
    [SerializeField] int money = 0;
    [SerializeField] int moneyToEnd = 50000; // when reach this, GOOD ending
    [SerializeField] TMP_Text moneyText;

    void Start()
    {
        daysLeft = DataSystem.Data.gameState.currentReplay;
        if (GameManager.Instance)
        {
            money = GameManager.Instance.playerMoney;
            moneyToEnd = GameManager.Instance.moneyToEnd;
        }

        if (money >= moneyToEnd)
        {
            // WIN game
        }
        else if (daysLeft <= 0)
        {
            // LOSE game
        }

        daysText.text = daysLeft.ToString();
        moneyText.text = "$" + money.ToString();
        SetRotation();
    }

    private void Update()
    {
        daysText.text = daysLeft.ToString();
        SetRotation();
    }

    void SetRotation()
    {
        float rotAmount = daysTotal / daysLeft;
        rotAmount = Mathf.Min(11, rotAmount);
        transform.localRotation = Quaternion.Euler(0, 0, rotAmount);
    }
}
