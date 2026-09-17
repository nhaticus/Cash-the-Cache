using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    [System.Serializable]
    public class Scene
    {
        public Sprite sprite;
        public string sfx;
    }

    [Header("Audio")]
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string music;

    [Header("UI")]
    [SerializeField] float timeToChange = 4.5f;
    [SerializeField] Image background, img1, img2;
    int currentImg = 1;

    [Header("Cutscenes")]
    [SerializeField] Scene[] scenes;
    int sceneCount = 0;
    bool canSwitchScene = false;

    // send event when finished
    public event Action CutsceneFinished;

    void Start()
    {
        singleAudio.PlayMusic(music, loop: true);

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
    }

    float timePassed = 0;
    void Update()
    {
        // every timeToChange seconds, change scene
        // when reached end, send cutsceneFinished event
        if(sceneCount <= scenes.Length && canSwitchScene)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= timeToChange)
            {
                StartCoroutine(BeginNextSceneChange());
                timePassed = 0;
            }
        }
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
        img1.sprite = scenes[0].sprite;
        img2.sprite = scenes[1].sprite;
        sceneCount = 2;

        StartCoroutine(FadeIn(background, 1.25f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeIn(img1, 1));
        singleAudio.PlaySFX(scenes[0].sfx);
        canSwitchScene = true;
    }

    /// <summary>
    /// Switch out img1/2 and change next image
    /// </summary>
    /// <returns></returns>
    IEnumerator BeginNextSceneChange()
    {
        canSwitchScene = false;
        if (currentImg == 1)
        {
            StartCoroutine(FadeIn(img2));
            yield return StartCoroutine(FadeOut(img1));
            if(sceneCount < scenes.Length)
            {
                img1.sprite = scenes[sceneCount].sprite;
                currentImg = 2;
            }
        }
        else
        {
            StartCoroutine(FadeIn(img1));
            yield return StartCoroutine(FadeOut(img2));

            if (sceneCount < scenes.Length)
            {
                img2.sprite = scenes[sceneCount].sprite;
                currentImg = 1;
            }
        }

        singleAudio.PlaySFX(scenes[sceneCount - 1].sfx);
        canSwitchScene = true;
        sceneCount++;

        if (sceneCount > scenes.Length)
        {
            CutsceneFinished.Invoke();
        }
    }

    IEnumerator FadeIn(Image image, float duration = 1.15f)
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

    IEnumerator FadeOut(Image image, float duration = 1.15f)
    {
        float elapsedTime = duration;
        Color originalColor = image.color;

        // Force starting alpha to 1 (fully visible)
        originalColor.a = 1f;
        image.color = originalColor;

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;

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
