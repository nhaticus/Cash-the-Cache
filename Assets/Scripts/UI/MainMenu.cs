using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject panel;

    [SerializeField] AudioMixer audioMixer;
    float defaultVolume = 1;

    [SerializeField] EventSystem eventSystem;
    [HideInInspector] public GameObject prevButton; // remember which button when renabling main menu

    private void Start()
    {
        // set master, music, and sfx volume
        audioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", defaultVolume)) * 20f);
        audioMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", defaultVolume)) * 20f);
        audioMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", defaultVolume)) * 20f);

        StartCoroutine(FadeOutEffect());
    }

    IEnumerator FadeOutEffect()
    {
        float fadeOutTime = 1.5f;
        panel.SetActive(true);
        CanvasGroup panelGroup = panel.GetComponent<CanvasGroup>();
        float time = fadeOutTime;
        while (time >= 0)
        {
            panelGroup.alpha = time / fadeOutTime;
            time -= Time.deltaTime;
            yield return null;
        }
        panel.SetActive(false);
    }

    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MenuRememberButton(GameObject button)
    {
        prevButton = button;
    }

}
