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

    public TextMeshProUGUI attemptsText; // Text to display remaining attempts
    public GameObject failedText;
    private GameObject currentDifficultyPanel;

    public int maxAttempts = 3;   // Maximum attempts before lock resets  
    private int currentAttempts = 0; // Tracks current attempts


    private int currentIndex = 0; // Tracks which pin should be pressed next
    private bool isLocked = false; // Prevent spam clicking
    private Vector3[] originalPositions; // Save original positions of pins

    private List<int> correctOrder = new List<int>(); // Stores the order of correct clicks

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    //Initializes the pins and sets up lock picking UI
    public void SetPins(GameObject difficultyPanel, string safeID) 
    {
        currentDifficultyPanel = difficultyPanel;

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

        LockPickingManager manager = FindObjectOfType<LockPickingManager>();
        if (manager != null)
        {
            List<int> savedOrder = manager.GetOrder(safeID);
            if (savedOrder != null)
            {
                correctOrder = new List<int>(savedOrder); // Use the saved order
                Debug.Log($"Loaded saved order for {safeID}: {string.Join(", ", correctOrder)}");
            }
            else
            {
                GenerateShuffledOrder(); // Generate a new order
                manager.SaveOrder(safeID, correctOrder); // Save the order
                Debug.Log($"Generated new order for {safeID}: {string.Join(", ", correctOrder)}");
            }
        }
        else
        {
            Debug.LogError("LockPickingManager not found!");
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

       // GenerateShuffledOrder();

        for (int i = 0; i < pins.Length; i++)
        {
            pinTransforms[i] = pins[i].GetComponent<RectTransform>();
            originalPositions[i] = pinTransforms[i].localPosition;

            int index = i;
            pins[i].onClick.RemoveAllListeners();  // Prevent duplicate events
            pins[i].onClick.AddListener(() => TryPressPin(index));
        }

    }
    // Exits the lock picking UI and resets the state.
    private void Exit() 
    {
        PlayerManager.Instance.unlockRotation();

        SetCursorState(false);

        if (currentDifficultyPanel != null)
        {
            currentDifficultyPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("currentDifficultyPanel is null in Exit()!");
        }

        // currentDifficultyPanel.SetActive(false);
        attemptsText.gameObject.SetActive(false);

        // Reset everything for the next use
        ResetAllPins();

        // Clear current order so it generates a new one next time or reload exisitng one
        correctOrder.Clear();
    }

    // Attempts to press a pin and checks if it is the correct one.
    void TryPressPin(int pinIndex) 
    {
        if (isLocked) return; // Prevent spam clicking


        LockPickingManager manager = FindObjectOfType<LockPickingManager>();

        // Check if the clicked pin is the next one in the correct order
        if (pinIndex == correctOrder[currentIndex])
        {
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++; // Move to next pin

            if (currentIndex >= pins.Length)
            {
                LockPickingManager lockManager = FindObjectOfType<LockPickingManager>();
                if (lockManager != null)
                {
                    lockManager.LockPickSuccess(); // Unlocks the specific safe
                }

                Debug.Log("Lock is picked successfully!");
                Exit();
            }
        }
        else
        {

            if (manager != null)
            {
                manager.ReduceAttempt();  // Reduce attempt count

                UpdateAttemptsUI(); //Ensure UI updates

                if (manager.currentAttempts <= 0) 
                {
                    StartCoroutine(ShowFailedMessage());
                    return;
                }
            }

            StartCoroutine(WrongPinEffect(pinIndex));
        }
    }

    // Applies a visual effect for a correctly pressed pin.
    private IEnumerator CorrectPinEffect(int pinIndex)
    {
        isLocked = true;

        // Change color to green
        pins[pinIndex].GetComponent<Image>().color = correctColor;

        // Move the pin slightly up
        Vector3 originalPos = pinTransforms[pinIndex].localPosition;
        pinTransforms[pinIndex].localPosition += new Vector3(0, 100f, 0); // Move up by 10

        yield return new WaitForSeconds(0.3f); // Small delay for effect

        isLocked = false;
    }

    // Applies a visual effect for an incorrectly pressed pin.
    IEnumerator WrongPinEffect(int pinIndex)
    {
        isLocked = true;

        // Change color to red
        Image pinImage = pins[pinIndex].GetComponent<Image>();
        pinImage.color = wrongColor;

        yield return new WaitForSeconds(1f); // Wait for 1 second

        // Reset color
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
            //reset pin color
            pins[i].GetComponent<Image>().color = defaultColor;

            //Reset pin position to original positon if moved up
            pinTransforms[i].localPosition = originalPositions[i];
        }

    }
    void GenerateShuffledOrder()
    {
        correctOrder.Clear();
        // Get the unique safe ID from LockPickTrigger
        LockPickTrigger safeTrigger = GetComponentInParent<LockPickTrigger>();
        string safeID = safeTrigger != null ? safeTrigger.safeID : "Unknown";

        List<int> indices = new List<int>();

        for (int i = 0; i < pins.Length; i++)
        {
            indices.Add(i);
        }

        System.Random rand = new System.Random();
        while (indices.Count > 0)
        {
            int randomIndex = rand.Next(indices.Count);
            correctOrder.Add(indices[randomIndex]); // Store the shuffled index
            indices.RemoveAt(randomIndex);
        }

        Debug.Log("Randomized Click Order: " + string.Join(", ", correctOrder));
    }

    void UpdateAttemptsUI()
    {
        LockPickingManager manager = FindObjectOfType<LockPickingManager>();
        if (manager != null && attemptsText != null)
        {
            attemptsText.text = "Attempts Left: " + manager.currentAttempts;
        }
    }

    // Displays a failed message and exits the lock picking UI.
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
