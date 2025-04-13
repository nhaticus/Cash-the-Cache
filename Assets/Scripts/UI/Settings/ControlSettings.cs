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
    [SerializeField] GameObject keyboardConfig, controllerConfig;

    [SerializeField] GameObject keyboardButton, controllerButton;
    GameObject selectedButton;

    [SerializeField] Color selectionButtonSelected;

    private void Start()
    {
        OpenKeyboard();
    }

    public void OpenKeyboard()
    {
        keyboardConfig.SetActive(true);
        controllerConfig.SetActive(false);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white;// reset previous selected button
        selectedButton = keyboardButton;
        selectedButton.GetComponent<Image>().color = selectionButtonSelected; // change to look selected
    }

    public void OpenController()
    {
        keyboardConfig.SetActive(false);
        controllerConfig.SetActive(true);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white;// reset previous selected button
        selectedButton = controllerButton;
        selectedButton.GetComponent<Image>().color = selectionButtonSelected; // change to look selected
    }
}
