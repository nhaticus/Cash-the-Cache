using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Attached to Settings in MainMenu scene
 * Sets a "cutscene" where each panel disappears and sound effects play
 */

public class HideClearData : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] EventSystem eventSystem;
    [SerializeField] SingleAudio singleAudio;

    [Header("UI Panels")]
    [SerializeField] GameObject panel1, panel2, main, settings;
    [SerializeField] GameObject settingsButton;

    public void HideObjects() {
        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(HideCutscene());
    }

    IEnumerator HideCutscene()
    {
        singleAudio.PlaySFX("explosion");
        singleAudio.PlaySFX("disappear");
        yield return FadeOutEffect(panel2, 0.9f);
        yield return new WaitForSeconds(0.5f);
        singleAudio.PlaySFX("disappear");
        yield return FadeOutEffect(panel1, 0.9f);
        yield return new WaitForSeconds(0.5f);
        main.SetActive(true);
        singleAudio.PlaySFX("disappear");
        yield return FadeOutEffect(settings, 0.9f);
        eventSystem.SetSelectedGameObject(settingsButton); // set selected button
    }

    IEnumerator FadeOutEffect(GameObject obj, float fadeOutTime)
    {
        obj.SetActive(true);
        CanvasGroup group = obj.GetComponent<CanvasGroup>();
        float time = fadeOutTime;
        while (time >= 0)
        {
            group.alpha = time / fadeOutTime;
            time -= Time.deltaTime;
            yield return null;
        }
        obj.SetActive(false);
    }
}
