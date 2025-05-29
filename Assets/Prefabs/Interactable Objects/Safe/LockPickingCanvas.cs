/*
 * LockPickingCanvas handles the lockpicking minigame logic for the safe
 * Player interacts with a set of pins that need to be pressed in a specific order to unlock the safe
 * Difficulty determines the number of pins and the complexity of the lockpicking
 * Dynamic pin creation and canvas creation
 * Each pin is assigned a random order to be pressed
 * Pins flash in the correct order to guide the player
 * Player has a limited number of attempts to unlock the safe
 * If the player fails and runs out of attempts then the safe is permantly locked and a message is shown
 * If the player successfully unlocks the safe, they can loot the contents inside
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LockPickingCanvas : MonoBehaviour
{
    [Header("Lock Picking Settings")]
    public int difficulty;
    private int maxAttempts = 0;
    public float pinSpacing = 50f; // Distance between pins
    [SerializeField] GameObject pinPrefab; // Prefab for the pin button
    [SerializeField] Transform pinsContainer; // Parent container for pins

    [Header("UI Settings")]
    public TextMeshProUGUI attemptsText;
    public GameObject failedText;
    [HideInInspector] public UnityEvent LockOpened;
    [HideInInspector] public UnityEvent LockFailed;

    [Header("Pin Puzzle Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private List<Button> pins = new List<Button>(); // List to store dynamically created pins
    private List<int> correctOrder = new List<int>(); // Correct order of pins to click
    private List<int> remainingPins = new List<int>(); // To keep track of pins that haven't been pressed yet
    private int currentIndex = 0;
    private RectTransform[] pinTransforms;
    private Color defaultColor;
    private Vector3[] originalPositions;
    private bool canClick = true;

    private void OnEnable()
    {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    private void Start()
    {
        CreatePins(); // Create pins dynamically based on difficulty
        UpdateAttemptsUI();
    }

    private void Tick()
    {
        if (maxAttempts <= 0)
        {
            UpdateAttemptsUI();

            StartCoroutine(ShowFailedMessage());
            LockFailed.Invoke();
        }
    }
    
    // Method to set max attempts from the Safe script
    public void SetMaxAttempts(int attempts)
    {
        maxAttempts = attempts;
    }
    // Dynamically creates pins based on the difficulty level
    private void CreatePins()
    {
        for (int i = 0; i < difficulty; i++)
        {
            Button newPin = Instantiate(pinPrefab, pinsContainer).GetComponent<Button>();
            float totalWidth = (difficulty - 1) * pinSpacing;
            float xOffset = -totalWidth / 2f; // Center the pins

            newPin.transform.localPosition = new Vector3(xOffset + i * pinSpacing, 0, 0);
            pins.Add(newPin);

            // Add listener for when the pin is clicked
            int pinIndex = i;
            newPin.onClick.AddListener(() => TryPressPin(pinIndex));
        }

        pinTransforms = new RectTransform[pins.Count];
        originalPositions = new Vector3[pins.Count];

        for (int i = 0; i < pins.Count; i++)
        {
            pinTransforms[i] = pins[i].GetComponent<RectTransform>(); // Assign each pin's RectTransform
            originalPositions[i] = pinTransforms[i].localPosition; // Save the initial position of each pin
        }

        defaultColor = pins[0].GetComponent<Image>().color;

        // Generate a random order for the pins to be clicked in
        GenerateShuffledOrder();
        StartCoroutine(AssignPinOrderEffect()); // Flash the pins in the correct order
    }

    // Randomly shuffle the pins for the player to follow the correct order
    private void GenerateShuffledOrder()
    {
        correctOrder.Clear();
        for (int i = 0; i < pins.Count; i++)
        {
            correctOrder.Add(i);
        }

        // Shuffle the list of correct pins order
        for (int i = 0; i < correctOrder.Count; i++)
        {
            int temp = correctOrder[i];
            int randomIndex = Random.Range(i, correctOrder.Count);
            correctOrder[i] = correctOrder[randomIndex];
            correctOrder[randomIndex] = temp;
        }
        remainingPins = new List<int>(correctOrder);
    }

    // Flash the pins in the correct order
    public IEnumerator AssignPinOrderEffect()
    {
        yield return new WaitForSeconds(1); // wait first 1 second
        for (int i = 0; i < pins.Count; i++)
        {
            // find order of pins[i]
            int order = 0;
            for (int j = 0; j < pins.Count; j++)
            {
                if (correctOrder[j] == i)
                {
                    order = j;
                    break;
                }
            }
            StartCoroutine(ShowPinOrder(i, order));
            yield return new WaitForSeconds((order * 0.5f) + 1f); // wait extra 1 seconds
        }
        yield return new WaitForSeconds(1); // wait 1 second before begin again
        StartCoroutine(AssignPinOrderEffect());
    }

    // Flash the pins to show their order
    IEnumerator ShowPinOrder(int pinIndex, int order)
    {
        Color faded = new Color(0.25f, 0.25f, 0.25f, 1); // Faded color for flashing effect
        for (int i = 0; i <= order; i++) // Flash the pin as many times as its order
        {
            if (!remainingPins.Contains(pinIndex)) // Skip flashing if already clicked
                yield break;

            pins[pinIndex].GetComponent<Image>().color = faded;
            yield return new WaitForSeconds(0.22f);
            pins[pinIndex].GetComponent<Image>().color = defaultColor;
            yield return new WaitForSeconds(0.22f);
        }
    }

    // Method for when the player clicks a pin
    private void TryPressPin(int pinIndex)
    {
        if (!canClick)
            return;
        if (pinIndex == correctOrder[currentIndex]) // If the pin clicked is correct
        {
                StartCoroutine(CorrectPinEffect(pinIndex));
                currentIndex++;
                remainingPins.Remove(pinIndex); // Remove from remainingPins so it stops flashing

                if (currentIndex >= pins.Count)
                {
                    LockOpened.Invoke(); // All pins clicked correctly, unlock the safe
                    ExitSafe();
                }
        }
        else
        {
                maxAttempts--;
                UpdateAttemptsUI(); // Update the attempts left UI
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
        yield return new WaitForSeconds(1.5f);
        ResetPins();
        canClick = true;
    }

    // Reset the pin colors and the order of interaction
    private void ResetPins()
    {
        currentIndex = 0;
        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].GetComponent<Image>().color = defaultColor;
            pinTransforms[i].localPosition = originalPositions[i];
        }
    }
    private void UpdateAttemptsUI()
    {
        if (attemptsText != null)
            attemptsText.text = $"Attempts Left: {maxAttempts}";
    }

    private IEnumerator ShowFailedMessage()
    {
        if (failedText) failedText.SetActive(true);
        yield return new WaitForSeconds(1);
        if (failedText) failedText.SetActive(false);

        ExitSafe();
    }

    public void ExitButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        // Disable the canvas instead of destroying it
        gameObject.SetActive(false); // This will keep the state of the pins and combo intact
    }

    public void ExitSafe()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject); // Destroy the canvas when exiting
    }
    public void StartFlashingOrder()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(AssignPinOrderEffect());
        }
    }
}
