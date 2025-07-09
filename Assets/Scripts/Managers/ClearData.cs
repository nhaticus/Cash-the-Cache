using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ClearData : MonoBehaviour
{
    [SerializeField] GameObject panel1, panel2;
    public void StartFadeInPanel(GameObject panel)
    {
        StartCoroutine(FadeInPanel(panel));
    }

    IEnumerator FadeInPanel(GameObject panel)
    {
        float fadeInTime = 0.5f;
        panel.SetActive(true);
        CanvasGroup panelGroup = panel.GetComponent<CanvasGroup>();
        float time = 0;
        while(time <= fadeInTime)
        {
            panelGroup.alpha = time / fadeInTime;
            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOutPanel(GameObject panel, float fadeInTime)
    {
        panel.SetActive(true);
        CanvasGroup panelGroup = panel.GetComponent<CanvasGroup>();
        float time = fadeInTime;
        while (time >= 0)
        {
            panelGroup.alpha = time / fadeInTime;
            time -= Time.deltaTime;
            yield return null;
        }
    }

    [SerializeField] string mainMenuScene = "Main Menu";
    IEnumerator ClearDataEffect()
    {
        // fade out each panel
        StartCoroutine(FadeOutPanel(panel2, 0.75f));
        yield return new WaitForSeconds(0.9f);

        StartCoroutine(FadeOutPanel(panel1, 0.75f));
        yield return new WaitForSeconds(0.9f);

        // restart main menu scene
        SceneManager.LoadScene(mainMenuScene);
    }
    void Clear()
    {
        int colorblind = PlayerPrefs.GetInt("ColorblindMode"); // keep colorblind
        PlayerPrefs.DeleteAll();
        DataSystem.ResetItems();
        GameManager.Instance.playerMoney = 0;
        PlayerPrefs.SetInt("ColorblindMode", colorblind); // restore colorblind
    }

    public void StartClearEffect()
    {
        Clear();
        StartCoroutine(ClearDataEffect());
    }
}
