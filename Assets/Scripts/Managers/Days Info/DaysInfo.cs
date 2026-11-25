using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DaysInfo : MonoBehaviour
{
    [SerializeField] float maxRotation = 11;

    [Header("Days")]
    [SerializeField] int daysLeft = 45; // when reach 0, BAD ending
    int daysTotal = 45;
    [SerializeField] TMP_Text daysText;

    [Header("Money")]
    [SerializeField] int money = 0;
    int moneyToEnd = 10000; // when reach this, GOOD ending
    [SerializeField] TMP_Text moneyText, moneyRequireText;

    [Header("Cutscene")]
    [SerializeField] Transform canvasLocation;
    [SerializeField] GameObject winScene, loseScene;
    [SerializeField] string atEndScene;
    [SerializeField] SingleAudio singleAudio; // stop music

    [Header("Van")]
    [SerializeField] GameObject van; // stop van from doing anything on cutscene
    CarController carController;
    BuildingDetection buildingDetection;

    void Start()
    {
        int currentDays = DataSystem.Data.gameState.currentReplay;
        int daysTotal = (int)DataSystem.GetItem("DaysTotal").statValue;
        daysLeft = daysTotal - currentDays;
        daysText.text = daysLeft.ToString();

        if (GameManager.Instance)
            money = GameManager.Instance.playerMoney;
        moneyToEnd = (int)DataSystem.GetItem("MoneyToEnd").statValue;
        moneyText.text = "$" + money.ToString();
        moneyRequireText.text = "$" + moneyToEnd.ToString();

        SetRotation();
    }

    private void Update()
    {
        daysText.text = daysLeft.ToString();
        moneyText.text = "$" + money.ToString();
        moneyRequireText.text = "$" + moneyToEnd.ToString();
        SetRotation();
    }

    void SetRotation()
    {
        float rotAmount;
        if (daysLeft > 0)
            rotAmount = daysTotal / daysLeft;
        else
            rotAmount = maxRotation; // prevent dividing by 0

        rotAmount = Mathf.Min(maxRotation, rotAmount);
        transform.localRotation = Quaternion.Euler(0, 0, rotAmount);
    }

    void DisableVan()
    {
        carController = van.GetComponentInChildren<CarController>();
        buildingDetection = van.GetComponentInChildren<BuildingDetection>();

        carController.canDrive = false;
        buildingDetection.canInteract = false;
    }

}
