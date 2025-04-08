using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LockPickingCanvas : MonoBehaviour
{
    public int difficulty;
    [HideInInspector] public UnityEvent LockOpened;

    [SerializeField] GameObject pinPrefab; // Prefab for the pin button
    [SerializeField] Transform pinsContainer; // Parent container for pins

    List<Button> pins = new List<Button>(); // List to store dynamically created pins
    List<int> correctOrder = new List<int>(); // Correct order of pins to click
    int currentIndex = 0;

    float spacing = 50f; // Distance between pins

    bool canClick = true;
    RectTransform[] pinTransforms;
    
    [Header("Pin Puzzle Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    Color defaultColor;
    Vector3[] originalPositions;

    private void Start()
    {
        CreatePins(); // Create pins dynamically based on difficulty
    }

    private void Update()
    {
        if (UserInput.Instance.Pause)
        {
            ExitSafe();
        }
    }

    // Dynamically creates pins based on the difficulty level
    private void CreatePins()
    {
        for (int i = 0; i < difficulty; i++)
        {
            Button newPin = Instantiate(pinPrefab, pinsContainer).GetComponent<Button>();
            newPin.transform.localPosition = new Vector3(i * spacing, 0, 0); // Place pins with space between them
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

    // Flash the pins in the correct order
    IEnumerator AssignPinOrderEffect()
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
       // if (isUnlocked)
            //yield break;
        yield return new WaitForSeconds(1); // wait 1 second before begin again
        StartCoroutine(AssignPinOrderEffect());
    }

    // Flash the pins to show their order
    IEnumerator ShowPinOrder(int pinIndex, int order)
    {
        Color faded = new Color(0.25f, 0.25f, 0.25f, 1); // Faded color for flashing effect
        for (int i = 0; i <= order; i++) // Flash the pin as many times as its order
        {
            pins[pinIndex].GetComponent<Image>().color = faded;
            yield return new WaitForSeconds(0.22f);
            pins[pinIndex].GetComponent<Image>().color = defaultColor;
            yield return new WaitForSeconds(0.22f);
            if (currentIndex > order) // If the user has correctly selected this pin, stop flashing
            {
                yield break;
            }
        }

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

        Debug.Log($"Generated Lock Combo: {string.Join(", ", correctOrder)}");
    }

    // Method for when the player clicks a pin
    private void TryPressPin(int pinIndex)
    {
        if (pinIndex == correctOrder[currentIndex]) // If the pin clicked is correct
        {
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++;
            if (currentIndex >= pins.Count)
            {
                LockOpened.Invoke(); // All pins clicked correctly, unlock the safe
                ExitSafe();
            }
        }
        else
        {
            StartCoroutine(WrongPinEffect(pinIndex));
            Debug.Log("Incorrect Pin! Try again.");
            //ResetPins(); // Reset pins if wrong pin is clicked
        }
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

    public void ExitSafe()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject); // Destroy the canvas when exiting
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
        ResetPins();
        canClick = true;
    }
}
