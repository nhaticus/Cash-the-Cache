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
    public GameObject findCombinationText;

    private Button[] pins;
    private RectTransform[] pinTransforms;
    private Color defaultColor;
    private Vector3[] originalPositions;
    private bool canClick = false;
    private int currentIndex = 0;
    private List<int> correctOrder = new List<int>();

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

        lockpickingUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Choose difficulty panel
        GameObject chosenPanel = null;
        if (difficulty == "Easy") chosenPanel = easyPanel;
        else if (difficulty == "Medium") chosenPanel = mediumPanel;
        else if (difficulty == "Hard") chosenPanel = hardPanel;

        chosenPanel.SetActive(true);

        SetupPins(chosenPanel);
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

        // Show attempts
        attemptsText.gameObject.SetActive(true);
        UpdateAttemptsUI();

        findCombinationText.SetActive(true);

        currentIndex = 0;
        canClick = false;

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
        canClick = true;
        pins[pinIndex].GetComponent<Image>().color = correctColor;
        pinTransforms[pinIndex].localPosition += new Vector3(0, 100f, 0);
        yield return new WaitForSeconds(0.3f);
        canClick = false;
    }

    private IEnumerator WrongPinEffect(int pinIndex)
    {
        canClick = true;
        Image pinImage = pins[pinIndex].GetComponent<Image>();
        pinImage.color = wrongColor;
        yield return new WaitForSeconds(1f);
        ResetAllPins();
        canClick = false;
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
        findCombinationText.SetActive(false);

        lockpickingUI.SetActive(false);
        ResetAllPins();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.setMoveSpeed(PlayerManager.Instance.getMaxMoveSpeed());
        PlayerManager.Instance.ableToInteract = true;
    }
}
