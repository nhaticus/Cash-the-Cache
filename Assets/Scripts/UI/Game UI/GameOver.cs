using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [Header("Money")]
    [SerializeField] int decreaseMoneyAmount = 2000;
    [SerializeField] float timeToDecrease = 1.75f;
    [SerializeField] float smackTime = 0.3f;

    [Header("Text")]
    [SerializeField] TMP_Text penaltyText;
    [SerializeField] TMP_Text currentMoneyText;
    [SerializeField] LocalizeStringEvent currentMoneyTranslation;

    [SerializeField] SingleAudio singleAudio;

    public void DecreasePlayerMoney()
    {
        if (GameManager.Instance == false)
            return;

        GameManager.Instance.playerMoney -= decreaseMoneyAmount;
        if (GameManager.Instance.playerMoney < 0)
            GameManager.Instance.playerMoney = 0;
    }

    public void PlayerLose()
    {
        if (GameManager.Instance == false)
            return;

        StartCoroutine(DisplayMoneyChanges());
    }

    // "roll" money lost, then "smack" total amount left
    IEnumerator DisplayMoneyChanges()
    {
        // hide total money and show when finished calculating
        currentMoneyText.alpha = 0;
        yield return StartCoroutine(RollMoneyDisplay());
        yield return new WaitForSeconds(0.65f);
        yield return StartCoroutine(SmackTotalAmount(GameManager.Instance.playerMoney));
    }

    // show how much money lost by "rolling"
    IEnumerator RollMoneyDisplay()
    {
        singleAudio.PlaySFX("Roll", loop: true);
        int currDecrease = 0;
        int rollAmount = 10;

        float time = 0;
        while(time < timeToDecrease)
        {
            time += Time.deltaTime;

            // set text and slightly rotate
            currDecrease = (int) ((time / timeToDecrease) * decreaseMoneyAmount);
            penaltyText.text = "-$" + currDecrease.ToString();

            float rotAmount = Random.Range(-9f, 9f);
            penaltyText.transform.localRotation = Quaternion.Euler(0, 0, rotAmount);
            yield return null;

            // if the rollAmount is too big that it will go over the total amount lost, set number to difference
            if(currDecrease + rollAmount > decreaseMoneyAmount)
            {
                rollAmount = decreaseMoneyAmount - currDecrease;
            }
        }

        penaltyText.transform.localRotation = Quaternion.Euler(0, 0, 0);
        singleAudio.StopSelectSFX("Roll");
    }

    IEnumerator SmackTotalAmount(int money)
    {
        currentMoneyText.transform.localScale = new Vector3(2.75f, 2.75f, 2.75f);
        currentMoneyTranslation.StringReference["money"] = new StringVariable { Value = money.ToString() };
        currentMoneyTranslation.RefreshString();
        currentMoneyText.alpha = 1;

        float currMoneySize = currentMoneyText.transform.localScale.x;

        float time = smackTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            currMoneySize = time + 1 / 1;
            currentMoneyText.transform.localScale = new Vector3(currMoneySize, currMoneySize, currMoneySize);
            yield return null;
        }

        singleAudio.PlaySFX("Smack");
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
