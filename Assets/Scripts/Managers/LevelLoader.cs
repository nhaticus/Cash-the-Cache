using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*
 * Loading screen for Level Gen
 * Waits for RoomGenerator's roomsFinished event to stop loading
 */

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject gear;
    [SerializeField] float spinSpeed = 100;

    bool finishLoading = false;
    public UnityEvent loadingComplete;

    private void Start()
    {
        StartCoroutine(RotateGear());
        StopPlayer();
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

    [SerializeField] PlayerMovement playerMovement;
    void StopPlayer()
    {
        playerMovement.canMove = false;
    }

    public void EndLoading()
    {
        loadingComplete.Invoke();
        finishLoading = true;

        StartCoroutine(RotateCanvas());
    }

    [SerializeField] GameObject pivot;
    [SerializeField] float unloadSpeed = 80;

    /// <summary>
    /// Rotate canvas so it feels like door is opening
    /// </summary>
    IEnumerator RotateCanvas()
    {
        while (pivot.transform.eulerAngles.y < 100)
        {
            pivot.transform.Rotate(0, unloadSpeed * Time.deltaTime, 0);
            yield return null;
        }
        gameObject.SetActive(false);
    }

}
