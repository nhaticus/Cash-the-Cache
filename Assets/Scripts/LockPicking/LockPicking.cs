using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockPicking : MonoBehaviour, InteractEvent
{
    public bool isUnlocked = false;
    public Animator safeAnimator;
    public int maxAttempts = 3;
    int currentAttempts;

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

    Button[] pins;
    RectTransform[] pinTransforms;
    Color defaultColor;
    Vector3[] originalPositions;
    int currentIndex = 0;
    List<int> correctOrder = new List<int>();
    bool canClick = true;

    [SerializeField] GameObject[] obj;

    private void Start()
    {
        currentAttempts = maxAttempts;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLockpickingOpen)
        {
            ExitLockpicking();
        }
    }

    public void Interact()
    {
        if (currentAttempts <= 0)
        {
            Debug.Log($"No attempts left for safe!");
            return;
        }

        isLockpickingOpen = true;
        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        lockpickingUI.SetActive(true);

        // Choose difficulty panel
        GameObject chosenPanel = null;
        if (difficulty == "Easy") chosenPanel = easyPanel;
        else if (difficulty == "Medium") chosenPanel = mediumPanel;
        else if (difficulty == "Hard") chosenPanel = hardPanel;

        chosenPanel.SetActive(true);
        SetupPins(chosenPanel);

        currentIndex = 0;
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

        if (correctOrder.Count == 0)
        {
            GenerateShuffledOrder();
        }
        else
        {
            Debug.Log($"Loaded saved combo: {string.Join(", ", correctOrder)}");
        }

        UpdateAttemptsUI();

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
        if (!canClick)
            return;
        Debug.Log($"Pin {pinIndex} clicked, expected: {correctOrder[currentIndex]} (step {currentIndex + 1}/{correctOrder.Count})");
        currentAttempts--;
        if (pinIndex == correctOrder[currentIndex])
        {
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++;
            if (currentIndex >= pins.Length)
            {
                MarkSafeUnlocked();
                ExitLockpicking();
            }
        }
        else
        {
            UpdateAttemptsUI();
            if (currentAttempts <= 0)
            {
                StartCoroutine(ShowFailedMessage());
                return;
            }
            StartCoroutine(WrongPinEffect(pinIndex));
        }
    }

    private IEnumerator CorrectPinEffect(int pinIndex)
    {
        canClick = false;
        pins[pinIndex].GetComponent<Image>().color = correctColor;
        pinTransforms[pinIndex].localPosition += new Vector3(0, 100f, 0);
        yield return new WaitForSeconds(0.3f);
        canClick = true;
    }

    private IEnumerator WrongPinEffect(int pinIndex)
    {
        canClick = false;
        pins[pinIndex].GetComponent<Image>().color = wrongColor;
        yield return new WaitForSeconds(1f);
        ResetAllPins();
        canClick = true;
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
        Debug.Log($"Generated Lock Combo: {string.Join(", ", correctOrder)}");
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
        MarkLocked();
        ExitLockpicking();
    }

    [SerializeField] Renderer safe;
    private void MarkSafeUnlocked()
    {
        isUnlocked = true;
        if (safeAnimator) safeAnimator.SetTrigger("OpenSafe");

        Instantiate(obj[UnityEngine.Random.Range(0, obj.Length - 1)], transform.position, transform.rotation);
        GetComponent<Renderer>().material.color = Color.green;
        safe.material.color = Color.green;
    }

    private void MarkLocked()
    {
        GetComponent<Renderer>().material.color = Color.yellow;
        safe.material.color = Color.yellow;
    }

    private void ExitLockpicking()
    {
        isLockpickingOpen = false;

        lockpickingUI.SetActive(false);
        ResetAllPins();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        PlayerManager.Instance.ableToInteract = true;
    }
}
