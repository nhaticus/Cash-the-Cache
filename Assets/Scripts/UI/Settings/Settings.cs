using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Holder for various settings
 * Does not actually change the settings
 * Volume
 * Control
 * MISC (ie: clearing data)
 */

public class Settings : MonoBehaviour
{
    [SerializeField]
    GameObject volumeSetting,
        controlSetting,
        miscSetting;

    [SerializeField]
    Button volumeButton,
        controlButton,
        miscButton;

    [SerializeField]
    Button volumeDownTargetButton,
        controlDownTargetButton,
        miscDownTargetButton;

    Button selectedButton;

    [SerializeField]
    Color selectionButtonSelected;

    private void Start()
    {
        OpenVolume();
    }

    public void OpenVolume()
    {
        volumeSetting.SetActive(true);
        controlSetting.SetActive(false);
        miscSetting.SetActive(false);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white; // reset previous selected button
        selectedButton = volumeButton;
        selectedButton.GetComponent<Image>().color = selectionButtonSelected; // change to look selected

        // Update new down button target
        Navigation nav = volumeButton.navigation;
        nav.selectOnDown = volumeDownTargetButton;
        volumeButton.navigation = nav;
    }

    public void OpenControls()
    {
        volumeSetting.SetActive(false);
        controlSetting.SetActive(true);
        miscSetting.SetActive(false);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white; // reset previous selected button
        selectedButton = controlButton;
        selectedButton.GetComponent<Image>().color = selectionButtonSelected; // change to look selected

        // Update new down button target
        Navigation nav = controlButton.navigation;
        nav.selectOnDown = controlDownTargetButton;
        controlButton.navigation = nav;
    }

    public void OpenMISC()
    {
        volumeSetting.SetActive(false);
        controlSetting.SetActive(false);
        miscSetting.SetActive(true);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white; // reset previous selected button
        selectedButton = miscButton;
        selectedButton.GetComponent<Image>().color = selectionButtonSelected; // change to look selected

        // Update new down button target
        Navigation nav = miscButton.navigation;
        nav.selectOnDown = miscDownTargetButton;
        miscButton.navigation = nav;
    }
}
