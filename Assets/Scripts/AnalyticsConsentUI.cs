using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Analytics;
using Unity.Services.Core;
using System.Threading.Tasks;

public class AnalyticsConsentUI : MonoBehaviour
{

    public Button yesButton;
    public Button noButton;
    public Button privacyPolicyButton;

    // Start is called before the first frame update
    void Start()
    {
        UnityServices.InitializeAsync();
        yesButton.onClick.AddListener(OnAccept);
        noButton.onClick.AddListener(OnDecline);
        privacyPolicyButton.onClick.AddListener(() =>
        {
            Application.OpenURL(AnalyticsService.Instance.PrivacyUrl);
        });
    }
    private async void OnAccept()
    {
        Debug.Log("Accepted");
        await AnalyticsManager.Instance.StartDataCollection();
        //AnalyticsManager.Instance.TrackGameStarted(); // Optional
        AnalyticsManager.Instance.TrackDeviceSpecs();
        gameObject.SetActive(false);
    }

    private void OnDecline()
    {
        Debug.Log("Declined"); 
        AnalyticsService.Instance.StopDataCollection();
        // Don't initialize analytics
        gameObject.SetActive(false);
    }

    private async void OpenPrivacyPolicy()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        Application.OpenURL(AnalyticsService.Instance.PrivacyUrl);
    }
}
