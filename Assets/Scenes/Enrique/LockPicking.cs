using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockPicking : MonoBehaviour
{
    public Button[] pins;        
    public RectTransform[] pinTransforms; 
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    private Color defaultColor;

    public int maxAttempts = 3;   // Maximum attempts before lock resets  
    private int currentAttempts = 0; // Tracks current attempts


    private int currentIndex = 0; // Tracks which pin should be pressed next
    private bool isLocked = false; // Prevent spam clicking
    private Vector3[] originalPositions; // Save original positions of pins

    private List<int> correctOrder = new List<int>(); // Stores the order of correct clicks

    public void SetPins(GameObject difficultyPanel) 
    {
        // Find the "Pins" container inside the difficulty panel
        Transform pinsContainer = difficultyPanel.transform.Find("Pins");



        // Get all buttons inside the "Pins" container
        pins = pinsContainer.GetComponentsInChildren<Button>();


        Debug.Log("Pins assigned: " + pins.Length);

        // Reset index
        currentIndex = 0;

        // Save default color
        defaultColor = pins[0].GetComponent<Image>().color;

        // Assign pinTransforms and click events
        pinTransforms = new RectTransform[pins.Length];
        originalPositions = new Vector3[pins.Length];

        GenerateShuffledOrder();

        for (int i = 0; i < pins.Length; i++)
        {
            pinTransforms[i] = pins[i].GetComponent<RectTransform>();
            originalPositions[i] = pinTransforms[i].localPosition;

            int index = i;
            pins[i].onClick.RemoveAllListeners();  // Prevent duplicate events
            pins[i].onClick.AddListener(() => TryPressPin(index));
        }

    }

    private void Exit() 
    {
        PlayerManager.Instance.unlockRotation();
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide UI
        gameObject.SetActive(false);

        // Reset everything for the next use
        ResetAllPins();

        // Clear current order so it generates a new one next time
        correctOrder.Clear();
    }
    void TryPressPin(int pinIndex) 
    {
        if (isLocked) return; // Prevent spam clicking

        Debug.Log("Clicked Pin: " + pinIndex + " | Expected: " + correctOrder[currentIndex]);

        // Check if the clicked pin is the next one in the correct order
        if (pinIndex == correctOrder[currentIndex])
        {
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++; // Move to next pin

            if (currentIndex >= pins.Length)
            {
                LockPickTrigger lockTrigger = FindObjectOfType<LockPickTrigger>();
                if (lockTrigger != null)
                {
                    lockTrigger.MarkSafeUnlocked();
                }

                Debug.Log("Lock is picked successfully!");
                Exit();
            }
        }
        else
        {
            
            StartCoroutine(WrongPinEffect(pinIndex));
        }
    }

    IEnumerator CorrectPinEffect(int pinIndex)
    {
        isLocked = true;

        // Change color to green
        pins[pinIndex].GetComponent<Image>().color = correctColor;

        // Move the pin slightly up
        Vector3 originalPos = pinTransforms[pinIndex].localPosition;
        pinTransforms[pinIndex].localPosition += new Vector3(0, 10f, 0); // Move up by 10

        yield return new WaitForSeconds(0.3f); // Small delay for effect

        isLocked = false;
    }

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
}
