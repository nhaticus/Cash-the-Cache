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

    private int currentIndex = 0; // Tracks which pin should be pressed next
    private bool isLocked = false; // Prevent spam clicking
    private Vector3[] originalPositions; // Save original positions of pins

 
    void Start()
    {

        // Save default color
        defaultColor = pins[0].GetComponent<Image>().color;


        //Save original positions
        originalPositions = new Vector3[pins.Length];
        for (int i = 0; i < pins.Length; i++) 
        {
            originalPositions[i] = pinTransforms[i].localPosition;
        }


        //Assign click event to each pin
        for (int i = 0; i < pins.Length; i++)
        {
            int index = i; // Prevent closure issue
            pins[i].onClick.AddListener(() => TryPressPin(index));
        }
    }
    private void Exit() 
    {
        PlayerManager.Instance.unlockRotation();
       
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void TryPressPin(int pinIndex) 
    {
        if (isLocked) return; // Prevent spam clicking

        if (pinIndex == currentIndex)
        {
            StartCoroutine(CorrectPinEffect(pinIndex));
            currentIndex++; // Move to next pin

            if (currentIndex >= pins.Length)
            {
                Debug.Log("Lock is picked successfully!");
                
            }
        }
        else
        {
            StartCoroutine(WrongPinEffect(pinIndex));
            //currentIndex = 0; // Reset
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
