using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    // display image and play sound effect
    [System.Serializable]
    public class Scene
    {
        public Sprite sprite;
        public string sfx;
    }

    [SerializeField] SingleAudio singleAudio;
    [SerializeField] float timeToChange = 2;
    [SerializeField] Image background, img1, img2;
    int currentImg = 1;

    [SerializeField] Scene[] scenes;
    int sceneCount = 0;

    // send event when enough Rooms Generated
    public UnityEvent cutsceneFinished;


    void Start()
    {
        // set img1 and img2 to first and second scene images
        img1.sprite = scenes[0].sprite;
        img2.sprite = scenes[1].sprite;
        sceneCount = 2;

        // set all images 0 alpha
        Color bgColor = background.color;
        bgColor.a = 0f;
        background.color = bgColor;

        Color img1Color = img1.color;
        img1Color.a = 0f;
        img1.color = img1Color;

        Color img2Color = img2.color;
        img2Color.a = 0f;
        img2.color = img2Color;

        Initialize();
    }

    void Update()
    {
        // every timeToChange, changeSceneImage()
    }

    /// <summary>
    /// fade into black, set images, then start to show the first image
    /// </summary>
    public void Initialize()
    {
        StartCoroutine(BeginInitialize());
    }

    IEnumerator BeginInitialize()
    {
        yield return FadeIn(background, 1.25f);
        yield return new WaitForSeconds(0.4f);
        yield return FadeIn(img1, 1);
    }

    /// <summary>
    /// Fade out one image, fade in the other
    /// When faded out image is done, set image to next available scene image
    /// </summary>
    public void ChangeSceneImage()
    {
        if (sceneCount >= scenes.Length)
            return;

        StartCoroutine(BeginNextSceneChange());

        sceneCount++;
    }

    /// <summary>
    /// Switch out img1/2 and change next image
    /// </summary>
    /// <returns></returns>
    IEnumerator BeginNextSceneChange()
    {
        if (currentImg == 1)
        {
            StartCoroutine(FadeIn(img2));
            yield return StartCoroutine(FadeOut(img1));
            img1.sprite = scenes[sceneCount].sprite;
        }
        else
        {
            StartCoroutine(FadeIn(img1));
            yield return StartCoroutine(FadeOut(img2));
            img2.sprite = scenes[sceneCount].sprite;
        }
    }

    IEnumerator FadeIn(Image image, float duration = 0.75f)
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;

        // Force starting alpha to 0 (fully transparent)
        originalColor.a = 0f;
        image.color = originalColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the current alpha value based on time ratio
            originalColor.a = Mathf.Clamp01(elapsedTime / duration);

            // Reassign the modified color structure back to the image
            image.color = originalColor;

            yield return null; // Wait until the next frame
        }

        originalColor.a = 1;
        image.color = originalColor;
    }

    IEnumerator FadeOut(Image image, float duration = 0.75f)
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;

        // Force starting alpha to 0 (fully transparent)
        originalColor.a = 0f;
        image.color = originalColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the current alpha value based on time ratio
            originalColor.a = Mathf.Clamp01(elapsedTime / duration);

            // Reassign the modified color structure back to the image
            image.color = originalColor;

            yield return null; // Wait until the next frame
        }

        originalColor.a = 0f;
        image.color = originalColor;
    }
}
