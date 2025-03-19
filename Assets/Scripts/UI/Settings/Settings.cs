using UnityEngine;
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
    [SerializeField] GameObject volumeSetting, controlSetting, miscSetting;
    
    [SerializeField] GameObject volumeButton, controlButton, miscButton;
    GameObject selectedButton;
    
    private void Start()
    {
        OpenVolume();
    }
    
    public void OpenVolume()
    {
        volumeSetting.SetActive(true);
        controlSetting.SetActive(false);
        miscSetting.SetActive(false);
        if(selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white;// reset previous selected button
        selectedButton = volumeButton;
        selectedButton.GetComponent<Image>().color = Color.red; // change to look selected
    }

    public void OpenControls()
    {
        volumeSetting.SetActive(false);
        controlSetting.SetActive(true);
        miscSetting.SetActive(false);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white;// reset previous selected button
        selectedButton = controlButton;
        selectedButton.GetComponent<Image>().color = Color.red; // change to look selected
    }

    public void OpenMISC()
    {
        volumeSetting.SetActive(false);
        controlSetting.SetActive(false);
        miscSetting.SetActive(true);
        if (selectedButton)
            selectedButton.GetComponent<Image>().color = Color.white;// reset previous selected button
        selectedButton = miscButton;
        selectedButton.GetComponent<Image>().color = Color.red; // change to look selected
    }
}
