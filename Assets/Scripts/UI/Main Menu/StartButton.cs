using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] Transform canvasLocation;
    [SerializeField] GameObject cutscene;
    [SerializeField] SingleAudio singleAudio; // stop music

    [Header("Main Functionality")]
    [SerializeField] string scene = "Map";
    
    public void OnClick() {
        /*
        if (DataSystem.Data.gameState.currentReplay > 0)
        {
            SwitchScene(scene);
        }
        else
        {
            GameObject cs = Instantiate(cutscene, canvasLocation);
            StartCoroutine(InitializeCutsceneWhenActive(cs));
        }
        */
        GameObject cs = Instantiate(cutscene, canvasLocation);
        StartCoroutine(InitializeCutsceneWhenActive(cs));
    }

    private IEnumerator InitializeCutsceneWhenActive(GameObject obj)
    {
        // Wait until the object is not null and active
        yield return new WaitUntil(() => obj != null && obj.activeInHierarchy);

        Cutscene cutscene = obj.GetComponent<Cutscene>();
        cutscene.Initialize();
        cutscene.CutsceneFinished += AtCutsceneEnd;

        singleAudio.StopAllMusic();
        singleAudio.StopAllSFX();
    }

    void AtCutsceneEnd()
    {
        StartCoroutine(AfterCutsceneSwitchScene());
    }

    IEnumerator AfterCutsceneSwitchScene()
    {
        yield return new WaitForSeconds(6);
        SwitchScene(scene);
    }

    public void SwitchScene(string gameScene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(gameScene);
    }
}
