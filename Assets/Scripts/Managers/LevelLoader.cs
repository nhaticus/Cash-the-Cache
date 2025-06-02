using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*
 * Loading screen for Level Gen
 * Waits for RoomGenerator's roomsFinished event to stop loading
 */

public class LevelLoader : MonoBehaviour
{
    [SerializeField] BrendanRooms generatorScript;
    [SerializeField] GameObject gear;
    [SerializeField] float spinSpeed = 100;

    bool finishLoading = false;
    public UnityEvent loadingComplete;

    private void Start()
    {
        generatorScript.roomsFinished += EndLoading;
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
        loadingComplete.Invoke();
        finishLoading = true;
        gameObject.SetActive(false);
    }

}
