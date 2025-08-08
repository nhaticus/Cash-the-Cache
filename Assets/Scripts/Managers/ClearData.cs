using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearData : MonoBehaviour
{
    [SerializeField] GameObject panel1, panel2;
    public void StartFadeInPanel(GameObject panel)
    {
        StartCoroutine(FadeInPanel(panel, 0.4f));
    }

    IEnumerator FadeInPanel(GameObject panel, float fadeInTime)
    {
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

    void Clear()
    {
        int colorblind = PlayerPrefs.GetInt("ColorblindMode"); // keep colorblind
        PlayerPrefs.DeleteAll();
        DataSystem.ResetItems();
        GameManager.Instance.playerMoney = 0;
        PlayerPrefs.SetInt("ColorblindMode", colorblind); // restore colorblind
    }

}
