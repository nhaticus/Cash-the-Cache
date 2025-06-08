using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
 * Loading screen for Level Gen
 * Waits for RoomGenerator's roomsFinished event to stop loading
 */

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Image littleVan;
    public UnityEvent loadingComplete;

    private void Start()
    {
        StopPlayer();
    }

    [SerializeField] PlayerMovement playerMovement;
    void StopPlayer()
    {
        playerMovement.canMove = false;
    }

    public void EndLoading()
    {
        loadingComplete.Invoke();

        StartCoroutine(FadeOutLittleVan());
        StartCoroutine(RotateCanvas());
    }

    float fadeTime = 0.5f;
    IEnumerator FadeOutLittleVan()
    {
        float timer = fadeTime;
        while(timer >= 0)
        {
            timer -= Time.deltaTime;
            Color color = littleVan.color;
            color.a = timer / fadeTime;
            littleVan.color = color;
            yield return null;
        }
        
    }

    [SerializeField] GameObject pivot;
    [SerializeField] float canvasRotateSpeed = 80;

    /// <summary>
    /// Rotate canvas so it feels like door is opening
    /// </summary>
    IEnumerator RotateCanvas()
    {
        while (pivot.transform.eulerAngles.y < 100)
        {
            pivot.transform.Rotate(0, canvasRotateSpeed * Time.deltaTime, 0);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
