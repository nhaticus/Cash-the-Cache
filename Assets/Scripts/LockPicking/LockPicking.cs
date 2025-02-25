using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockPicking : MonoBehaviour
{
    public Button[] pins;
    public RectTransform[] pinTransforms;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    private Color defaultColor;

    public TextMeshProUGUI attemptsText;
    public GameObject failedText;
    private GameObject currentDifficultyPanel;

    private int currentIndex = 0;
    private bool isLocked = false;
    private Vector3[] originalPositions;
    public bool isLockpicking = false;

    private List<int> correctOrder = new List<int>();

    // Reference to the safe that is being lockpicked
    private LockPickTrigger currentSafe;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLockpicking)
            {
                Exit();
            }
        }
    }

    // Initializes the pins and sets up the lock-picking UI.
    // Now it receives a reference to the safe instead of just a safeID.
    public void SetPins(GameObject difficultyPanel, LockPickTrigger safe)
    {
        currentSafe = safe;
        currentDifficultyPanel = difficultyPanel;
        isLockpicking = true;

        if (currentDifficultyPanel == null)
        {
            Debug.LogError("currentDifficultyPanel is NULL in SetPins!");
        }

        Debug.Log($"currentDifficultyPanel set to {currentDifficultyPanel.name}");

        // Find the "Pins" container inside the difficulty panel
        Transform pinsContainer = difficultyPanel.transform.Find("Pins");
        if (pinsContainer == null)
        {
            Debug.LogError("Pins container not found in the difficulty panel!");
            return;
        }

        // Get all buttons inside the "Pins" container
        pins = pinsContainer.GetComponentsInChildren<Button>();
        Debug.Log("Pins assigned: " + pins.Length);

        // Try to load a saved order from the safe.
        List<int> savedOrder = currentSafe.GetOrder();
        if (savedOrder != null)
        {
            correctOrder = new List<int>(savedOrder);
            Debug.Log($"Loaded saved order for {currentSafe.safeID}: {string.Join(", ", correctOrder)}");
        }
        else
        {
            GenerateShuffledOrder();
            currentSafe.SaveOrder(correctOrder);
            Debug.Log($"Generated new order for {currentSafe.safeID}: {string.Join(", ", correctOrder)}");
        }

        if (attemptsText != null)
        {
            attemptsText.gameObject.SetActive(true);
        }
        UpdateAttemptsUI();

        // Reset index
        currentIndex = 0;

        // Save default color
        defaultColor = pins[0].GetComponent<Image>().color;

        // Assign pinTransforms and click events
        pinTransforms = new RectTransform[pins.Length];
        originalPositions = new Vector3[pins.Length];

        for (int i = 0; i < pins.Length; i++)
        {
            pinTransforms[i] = pins[i].GetComponent<RectTransform>();
            originalPositions[i] = pinTransforms[i].localPosition;

            int index = i;
            pins[i].onClick.RemoveAllListeners();  // Prevent duplicate events
            pins[i].onClick.AddListener(() => TryPressPin(index));
        }
    }

    // Exits the lock-picking UI and resets state.
    private void Exit()
    {
        isLockpicking = false;
        PlayerManager.Instance.unlockRotation();
        if (currentSafe != null)
        {
            currentSafe.isLockpickingOpen = false;
        }

        LockPickTrigger.anyLockpickingOpen = false;



        SetCursorState(false);

        if (currentDifficultyPanel != null)
        {
            currentDifficultyPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("currentDifficultyPanel is null in Exit()!");
        }

        attemptsText.gameObject.SetActive(false);
        ResetAllPins();
        correctOrder.Clear();
    }

    // Attempts to press a pin and checks if it is correct.
    void TryPressPin(int pinIndex)
    {
        if (isLocked) return; // Prevent spam clicking

        // Log which pin was clicked and which one is currently expected
        Debug.Log($"[LockPicking] Pin pressed: {pinIndex}, " + $"expected pin index: {correctOrder[currentIndex]} (currentIndex = {currentIndex})");


        if (pinIndex == correctOrder[currentIndex])
        {
            Debug.Log($"[LockPicking] Pin {pinIndex} is CORRECT! Moving on to the next pin.");
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++; // Move to next pin

            if (currentIndex >= pins.Length)
            {
                if (currentSafe != null)
                {
                    currentSafe.MarkSafeUnlocked();
                    currentSafe = null;
                }

                Debug.Log("Lock is picked successfully!");
                Exit();
            }
            else 
            {
                // Now the puzzle expects correctOrder[currentIndex]
                Debug.Log($"[LockPicking] Next expected pin index is now: {correctOrder[currentIndex]} " + $"(currentIndex = {currentIndex}).");
            }
        }
        else
        {
            Debug.Log($"[LockPicking] Pin {pinIndex} is WRONG! The puzzle still expects pin index {correctOrder[currentIndex]}.");
            // Wrong pin pressed: reduce an attempt on the safe.
            if (currentSafe != null)
            {
                bool attemptLeft = currentSafe.ReduceAttempt();
                UpdateAttemptsUI();
                if (!attemptLeft)
                {
                    StartCoroutine(ShowFailedMessage());
                    return;
                }
            }

            StartCoroutine(WrongPinEffect(pinIndex));
        }
    }

    // Visual effect for a correct pin press.
    private IEnumerator CorrectPinEffect(int pinIndex)
    {
        isLocked = true;
        pins[pinIndex].GetComponent<Image>().color = correctColor;
        pinTransforms[pinIndex].localPosition += new Vector3(0, 100f, 0);
        yield return new WaitForSeconds(0.3f);
        isLocked = false;
    }

    // Visual effect for an incorrect pin press.
    IEnumerator WrongPinEffect(int pinIndex)
    {
        isLocked = true;
        Image pinImage = pins[pinIndex].GetComponent<Image>();
        pinImage.color = wrongColor;
        yield return new WaitForSeconds(1f);
        pinImage.color = defaultColor;
        ResetAllPins();
        isLocked = false;
    }

    // Resets all pins to their original state.
    void ResetAllPins()
    {
        currentIndex = 0;
        for (int i = 0; i < pins.Length; i++)
        {
            pins[i].GetComponent<Image>().color = defaultColor;
            pinTransforms[i].localPosition = originalPositions[i];
        }
    }

    // Generates a random (shuffled) order for the pins.
    void GenerateShuffledOrder()
    {
        correctOrder.Clear();
        string safeID = currentSafe != null ? currentSafe.safeID : "Unknown";
        List<int> indices = new List<int>();
        for (int i = 0; i < pins.Length; i++)
        {
            indices.Add(i);
        }

        System.Random rand = new System.Random();
        while (indices.Count > 0)
        {
            int randomIndex = rand.Next(indices.Count);
            correctOrder.Add(indices[randomIndex]);
            indices.RemoveAt(randomIndex);
        }
        Debug.Log("Randomized Click Order: " + string.Join(", ", correctOrder));
    }

    void UpdateAttemptsUI()
    {
        if (currentSafe != null && attemptsText != null)
        {
            attemptsText.text = "Attempts Left: " + currentSafe.currentAttempts;
        }
    }

    // Shows a failed message and then exits.
    IEnumerator ShowFailedMessage()
    {
        if (failedText != null)
        {
            failedText.SetActive(true);
        }
        else
        {
            Debug.LogError("FAILED! text is NULL! Assign it in the Inspector.");
        }
        yield return new WaitForSeconds(1f);
        if (failedText != null)
        {
            failedText.SetActive(false);
        }
        Exit();
    }

    private void SetCursorState(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
