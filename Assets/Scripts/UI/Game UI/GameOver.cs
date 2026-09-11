using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] int decreaseMoneyAmount = 1000;
    [SerializeField] TMP_Text penaltyText;
    [SerializeField] TMP_Text currentMoneyText;
    [SerializeField] LocalizeStringEvent currentMoneyTranslation;

    private void Start()
    {
        PlayerLose();
    }

    public void PlayerLose()
    {
        /*
        if (GameManager.Instance == false)
            return;

        GameManager.Instance.playerMoney -= decreaseMoneyAmount;
        if (GameManager.Instance.playerMoney < 0)
            GameManager.Instance.playerMoney = 0;
        */
        StartCoroutine(DisplayMoneyChanges());
    }

    // "roll" money lost, then "smack" total amount left
    IEnumerator DisplayMoneyChanges()
    {
        // hide total money and show when finished calculating
        currentMoneyText.alpha = 0;
        yield return StartCoroutine(RollMoneyDisplay());
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(SmackTotalAmount());
    }

    // show how much money lost by "rolling"
    IEnumerator RollMoneyDisplay()
    {
        int currDecrease = 0;
        int rollAmount = 10;
        while(currDecrease < decreaseMoneyAmount)
        {
            // set text and slightly rotate
            currDecrease += rollAmount;
            penaltyText.text = "-$" + currDecrease.ToString();

            float rotAmount = Random.Range(-8f, 8f);
            penaltyText.transform.localRotation = Quaternion.Euler(0, 0, rotAmount);
            yield return new WaitForSeconds(0.05f);

            // if the rollAmount is too big that it will go over the total amount lost, set number to difference
            if(currDecrease + rollAmount > decreaseMoneyAmount)
            {
                rollAmount = decreaseMoneyAmount - currDecrease;
                Debug.Log("Too big, change roll amount to: " + rollAmount);
            }
        }
        penaltyText.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    IEnumerator SmackTotalAmount()
    {
        currentMoneyText.transform.localScale = new Vector3(2, 2, 2);
        currentMoneyTranslation.StringReference["money"] = new StringVariable { Value = "5050" }; //GameManager.Instance.playerMoney.ToString();
        currentMoneyTranslation.RefreshString();
        currentMoneyText.alpha = 1;

        float currMoneySize = currentMoneyText.transform.localScale.x;
        while (currMoneySize > 1)
        {
            currMoneySize -= 0.175f;
            currentMoneyText.transform.localScale = new Vector3(currMoneySize, currMoneySize, currMoneySize);
            yield return new WaitForSeconds(0.02f);
        }
        currentMoneyText.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
