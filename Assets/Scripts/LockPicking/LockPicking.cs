using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;  
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockPicking : MonoBehaviour
{
    public static bool anyLockpickingOpen = false;

    public string safeID;
    public bool isUnlocked = false;
    public Animator safeAnimator;
    public int maxAttempts = 3;
    [HideInInspector] public int currentAttempts;

    public string difficulty = "Easy";
    public GameObject easyPanel;
    public GameObject mediumPanel;
    public GameObject hardPanel;

    public GameObject lockpickingUI;
    public bool isLockpickingOpen = false;

    [Header("Pin Puzzle Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public TextMeshProUGUI attemptsText;
    public GameObject failedText;

    private Button[] pins;
    private RectTransform[] pinTransforms;
    private Color defaultColor;
    private Vector3[] originalPositions;
    private bool isLocked = false;
    private int currentIndex = 0;
    private List<int> correctOrder = new List<int>();

    private void Start()
    {
        if (string.IsNullOrEmpty(safeID))
            safeID = gameObject.name;

        currentAttempts = maxAttempts;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLockpickingOpen)
        {
            ExitLockpicking();
        }
    }

    public void OpenLockpicking()
    {
        if (isUnlocked)
        {
            Debug.Log($"Safe {safeID} is already unlocked!");
            return;
        }
        if (isLockpickingOpen)
        {
            Debug.Log($"Lockpicking already in progress on {safeID}!");
            return;
        }
        if (currentAttempts <= 0)
        {
            Debug.Log($"No attempts left for safe {safeID}!");
            return;
        }

        PlayerManager.Instance.lockRotation();
        anyLockpickingOpen = true;
        isLockpickingOpen = true;

        lockpickingUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Choose difficulty panel
        if (easyPanel) easyPanel.SetActive(false);
        if (mediumPanel) mediumPanel.SetActive(false);
        if (hardPanel) hardPanel.SetActive(false);

        GameObject chosenPanel = null;
        if (difficulty == "Easy") chosenPanel = easyPanel;
        else if (difficulty == "Medium") chosenPanel = mediumPanel;
        else if (difficulty == "Hard") chosenPanel = hardPanel;

        if (chosenPanel == null)
        {
            Debug.LogError($"Difficulty '{difficulty}' panel not set or invalid!");
            return;
        }
        chosenPanel.SetActive(true);

        SetupPins(chosenPanel);
        Debug.Log($"Lock Picking UI opened for Safe: {safeID}, Difficulty: {difficulty}");
    }

    private void SetupPins(GameObject difficultyPanel)
    {
        Transform pinsContainer = difficultyPanel.transform.Find("Pins");
        if (pinsContainer == null)
        {
            Debug.LogError("Pins container not found!");
            return;
        }

        pins = pinsContainer.GetComponentsInChildren<Button>(true);
        if (pins.Length == 0)
        {
            Debug.LogError("No pin buttons found!");
            return;
        }
        Debug.Log($"Pins assigned: {pins.Length}");

        //Try to load saved combo, else generate
        LoadCombo();

        if (correctOrder.Count == 0) // means no saved data found
        {
            GenerateShuffledOrder();
            SaveCombo(); // store the new combo
        }
        else
        {
            Debug.Log($"Loaded saved combo for {safeID}: {string.Join(", ", correctOrder)}");
        }

        // Show attempts
        if (attemptsText != null)
            attemptsText.gameObject.SetActive(true);
        UpdateAttemptsUI();

        currentIndex = 0;
        isLocked = false;

        defaultColor = pins[0].GetComponent<Image>().color;

        pinTransforms = new RectTransform[pins.Length];
        originalPositions = new Vector3[pins.Length];

        for (int i = 0; i < pins.Length; i++)
        {
            pinTransforms[i] = pins[i].GetComponent<RectTransform>();
            originalPositions[i] = pinTransforms[i].localPosition;

            int index = i;
            pins[i].onClick.RemoveAllListeners();
            pins[i].onClick.AddListener(() => TryPressPin(index));
        }
    }
private void TryPressPin(int pinIndex)
    {
        if (isLocked) return;

        Debug.Log($"Pin {pinIndex} clicked, expected: {correctOrder[currentIndex]} (step {currentIndex + 1}/{correctOrder.Count})");

        if (pinIndex == correctOrder[currentIndex])
        {
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++;
            if (currentIndex >= pins.Length)
            {
                Debug.Log($"Safe {safeID} is unlocked!");
                MarkSafeUnlocked();
                ExitLockpicking();
            }
        }
        else
        {
            Debug.Log($"Pin {pinIndex} is wrong! expected {correctOrder[currentIndex]}");
            bool attemptLeft = ReduceAttempt();
            UpdateAttemptsUI();
            if (!attemptLeft)
            {
                StartCoroutine(ShowFailedMessage());
                return;
            }
            StartCoroutine(WrongPinEffect(pinIndex));
        }
    }

    private IEnumerator CorrectPinEffect(int pinIndex)
    {
        isLocked = true;
        pins[pinIndex].GetComponent<Image>().color = correctColor;
        pinTransforms[pinIndex].localPosition += new Vector3(0, 100f, 0);
        yield return new WaitForSeconds(0.3f);
        isLocked = false;
    }

    private IEnumerator WrongPinEffect(int pinIndex)
    {
        isLocked = true;
        Image pinImage = pins[pinIndex].GetComponent<Image>();
        pinImage.color = wrongColor;
        yield return new WaitForSeconds(1f);
        pinImage.color = defaultColor;
        ResetAllPins();
        isLocked = false;
    }

    private void ResetAllPins()
    {
        currentIndex = 0;
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].GetComponent<Image>().color = defaultColor;
            pinTransforms[i].localPosition = originalPositions[i];
        }
    }

    private void GenerateShuffledOrder()
    {
        correctOrder.Clear();
        int pinCount = pins.Length;
        List<int> indices = new List<int>();
        for (int i = 0; i < pinCount; i++) indices.Add(i);

        System.Random rand = new System.Random();
        while (indices.Count > 0)
        {
            int randomIndex = rand.Next(indices.Count);
            correctOrder.Add(indices[randomIndex]);
            indices.RemoveAt(randomIndex);
        }
        Debug.Log($"Generated Lock Combo for {safeID}: {string.Join(", ", correctOrder)}");
    }

    // ============ Save & Load to PlayerPrefs ============

    private void LoadCombo()
    {
        // e.g. key = "SafeCombo_" + safeID
        string key = "SafeCombo_" + safeID + "_" + difficulty;
        if (PlayerPrefs.HasKey(key))
        {
            string savedString = PlayerPrefs.GetString(key);
            // "2,0,1,3"
            string[] tokens = savedString.Split(',');
            correctOrder.Clear();
            foreach (string t in tokens)
            {
                if (int.TryParse(t, out int pinIndex))
                    correctOrder.Add(pinIndex);
            }
        }
    }

    private void SaveCombo()
    {
        string key = "SafeCombo_" + safeID + "_" + difficulty;
        string comboString = string.Join(",", correctOrder);
        PlayerPrefs.SetString(key, comboString);
        PlayerPrefs.Save();
        Debug.Log($"Saved combo for {safeID} at {difficulty}: {comboString}");
    }

    private bool ReduceAttempt()
    {
        currentAttempts--;
        Debug.Log($"Attempts left for {safeID}: {currentAttempts}");
        if (currentAttempts <= 0)
        {
            Debug.Log($"No attempts left for {safeID}!");
            return false;
        }
        return true;
    }

    private void UpdateAttemptsUI()
    {
        if (attemptsText != null)
            attemptsText.text = $"Attempts Left: {currentAttempts}";
    }

    private IEnumerator ShowFailedMessage()
    {
        if (failedText) failedText.SetActive(true);
        yield return new WaitForSeconds(1f);
        if (failedText) failedText.SetActive(false);
        ExitLockpicking();
    }

    private void MarkSafeUnlocked()
    {
        isUnlocked = true;
        Debug.Log($"Safe {safeID} Unlocked!");
        if (safeAnimator) safeAnimator.SetTrigger("OpenSafe");
    }

    private void ExitLockpicking()
    {
        isLockpickingOpen = false;
        anyLockpickingOpen = false;

        if (lockpickingUI) lockpickingUI.SetActive(false);
        ResetAllPins();

        // Lock the cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.unlockRotation();
        Debug.Log($"Lockpicking closed for {safeID}");
    }
}
