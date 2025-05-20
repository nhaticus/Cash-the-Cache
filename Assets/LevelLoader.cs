using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private GameObject levelGenerator;
    private IEnumerator Start()
    {
        loadingScreen.SetActive(true);

        RoomGenerator generatorScript = levelGenerator.GetComponent<RoomGenerator>();
        loadingSlider.value = 0f;
        
        float progress = 0f;

        while (progress < 0.6f)
        {
            progress += Time.deltaTime * 0.5f;
            loadingSlider.value = progress;
            yield return null;
        }
        while (progress < 0.9f && !generatorScript.isComplete)
        {
            progress += Time.deltaTime * 0.2f;
            loadingSlider.value = progress;
            yield return null;
        }
        while (!generatorScript.isComplete)
        {
            yield return null;
        }
        while (progress < 1f)
        {
            progress += Time.deltaTime * 1.5f;
            loadingSlider.value = progress;
            yield return null;
        }
        loadingScreen.SetActive(false);
    }

}
