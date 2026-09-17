using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Cutscene")]
    [SerializeField] Transform canvasLocation;
    [SerializeField] GameObject winScene, loseScene;
    [SerializeField] string atEndScene;
    [SerializeField] SingleAudio singleAudio; // stop music

    void Start()
    {
        daysLeft = DataSystem.Data.gameState.currentReplay;
        if (GameManager.Instance)
        {
            money = GameManager.Instance.playerMoney;
            moneyToEnd = GameManager.Instance.moneyToEnd;
        }

        if (money >= moneyToEnd) // WIN game
        {
            GameObject cs = Instantiate(winScene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
        }
        else if (daysLeft <= 0) // LOSE game
        {
            GameObject cs = Instantiate(loseScene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
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

    private IEnumerator InitializeCutsceneWhenActive(GameObject obj)
    {
        // Wait until the object is not null and active
        yield return new WaitUntil(() => obj != null && obj.activeInHierarchy);

        Cutscene cutscene = obj.GetComponent<Cutscene>();
        cutscene.Initialize();
        cutscene.CutsceneFinished += AtCutsceneEnd;

        singleAudio.StopAllMusic();
        singleAudio.StopAllSFX();
    }

    void AtCutsceneEnd()
    {
        StartCoroutine(AfterCutsceneSwitchScene());
    }

    IEnumerator AfterCutsceneSwitchScene()
    {
        yield return new WaitForSeconds(6);
        SwitchScene(atEndScene);
    }
    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }
}
