using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndCanvas : MonoBehaviour
{
    [Header("Days")]
    int daysLeft;
    [SerializeField] TMP_Text daysText;

    [Header("Money")]
    int money, moneyToEnd;
    [SerializeField] TMP_Text moneyText, moneyRequireText, moneyPercentageText;

    [Header("Cutscene")]
    [SerializeField] Transform canvasLocation;
    [SerializeField] GameObject winScene, loseScene;
    [SerializeField] string sceneAfterGameEnd;
    [SerializeField] SingleAudio singleAudio; // stop music

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        int currentDays = DataSystem.Data.gameState.currentReplay;
        int daysTotal = (int) DataSystem.GetItem("DaysTotal").statValue;
        daysLeft = daysTotal - currentDays;
        daysText.text = daysLeft.ToString();

        if (GameManager.Instance)
            money = GameManager.Instance.playerMoney;
        moneyToEnd = (int) DataSystem.GetItem("MoneyToEnd").statValue;
        moneyText.text = "$" + money.ToString();
        moneyRequireText.text = "$" + moneyToEnd.ToString();
        float percentage = (money / (float) moneyToEnd) * 100;
        moneyPercentageText.text = Mathf.Round(percentage * 10.0f) * 0.1f + "%";

        CheckConditions();
    }

    void CheckConditions()
    {
        if(daysLeft <= 0)
        {
            // lose
            ClearGameData();
            GameObject cs = Instantiate(loseScene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
        }
        else if(money >= moneyToEnd)
        {
            // win
            ClearGameData();
            GameObject cs = Instantiate(winScene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
        }
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
        yield return new WaitForSeconds(4);
        SwitchScene(sceneAfterGameEnd);
    }
    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }

    void ClearGameData()
    {
        if (GameManager.Instance == false)
            return;

        int colorblind = PlayerPrefs.GetInt("ColorblindMode"); // keep colorblind
        PlayerPrefs.DeleteAll();
        DataSystem.ResetItems();
        DataSystem.ResetData();
        GameManager.Instance.SetMoney(0);
        PlayerPrefs.SetInt("ColorblindMode", colorblind); // restore colorblind
    }

}
