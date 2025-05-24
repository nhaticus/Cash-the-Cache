using System.Collections;
using UnityEngine;

/*
 * Loading screen for Level Gen
 * Waits for RoomGenerator's roomsFinished event to stop loading
 */

public class LevelLoader : MonoBehaviour
{
    /*
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private GameObject levelGenerator;

    /*
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
    */

    [SerializeField] RoomGenerator generatorScript;
    [SerializeField] GameObject gear;
    [SerializeField] float spinSpeed = 60;
    bool finishLoading = false;

    private void Start()
    {
        generatorScript.roomsFinished.AddListener(EndLoading);
        StartCoroutine(RotateGear());
    }

    private IEnumerator RotateGear()
    {
        while (!finishLoading)
        {
            // rotate
            gear.transform.Rotate(0, 0, spinSpeed * -Time.deltaTime);
            yield return null;
        }
    }

    void EndLoading()
    {
        finishLoading = true;
        gameObject.SetActive(false);
    }

}
