using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Holder for Control Mapping settings
 * https://www.youtube.com/watch?v=qXbjyzBlduY&ab_channel=SasquatchBStudios
 */

public class ControlSettings : MonoBehaviour
{
    [SerializeField] GameObject keyboardConfig;

    [SerializeField] GameObject startKeyboardButton;
    GameObject selectedButton;

    [SerializeField] Color selectionButtonSelected;

    private void Start()
    {
        //OpenKeyboard();
    }

    public void OpenKeyboard()
    {
        keyboardConfig.SetActive(true);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white;// reset previous selected button
        selectedButton = startKeyboardButton;
        selectedButton.GetComponent<Image>().color = selectionButtonSelected; // change to look selected
    }
}
