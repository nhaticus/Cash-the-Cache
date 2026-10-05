using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class DaysInfo : MonoBehaviour
{
    [SerializeField] float maxRotation = 11;

    [Header("Days")]
    [SerializeField] int daysLeft = 45; // when reach 0, BAD ending
    [SerializeField] int daysTotal = 45;
    [SerializeField] TMP_Text daysText;

    [Header("Money")]
    [SerializeField] int money = 0;
    [SerializeField] int moneyToEnd = 10000; // when reach this, GOOD ending
    [SerializeField] TMP_Text moneyText;

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
        daysLeft = daysTotal - currentDays;
        if (GameManager.Instance)
        {
            money = GameManager.Instance.playerMoney;
            moneyToEnd = GameManager.Instance.moneyToEnd;
        }

        CheckConditions();

        daysText.text = daysLeft.ToString();
        moneyText.text = "$" + money.ToString();
        SetRotation();
    }

    bool test = false;
    private void Update()
    {
        if(test == false)
        {
            daysText.text = daysLeft.ToString();
            moneyText.text = "$" + money.ToString();
            SetRotation();

            CheckConditions();
        }
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

    void CheckConditions()
    {
        if (money >= moneyToEnd) // WIN game
        {
            GameObject cs = Instantiate(winScene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
            Clear();
            DisableVan();
            test = true;
        }
        else if (daysLeft <= 0) // LOSE game
        {
            GameObject cs = Instantiate(loseScene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
            Clear();
            DisableVan();
            test = true;
        }
    }

    void DisableVan()
    {
        carController = van.GetComponentInChildren<CarController>();
        buildingDetection = van.GetComponentInChildren<BuildingDetection>();

        carController.canDrive = false;
        buildingDetection.canInteract = false;
    }

    void Clear()
    {
        if (GameManager.Instance == false)
            return;

        int colorblind = PlayerPrefs.GetInt("ColorblindMode"); // keep colorblind
        PlayerPrefs.DeleteAll();
        DataSystem.ResetItems();
        DataSystem.ResetData();
        Debug.Log("days: " + DataSystem.Data.gameState.currentReplay);
        Debug.Log("money: " + GameManager.Instance.playerMoney);
        GameManager.Instance.SetMoney(0);
        Debug.Log("money: " + GameManager.Instance.playerMoney);
        PlayerPrefs.SetInt("ColorblindMode", colorblind); // restore colorblind
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
