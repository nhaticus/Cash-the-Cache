using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopVan : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject canvasPrefab;
    [SerializeField] string levelScene = "Main Level";
    public void Interact()
    {
        StartCoroutine(LeaveLevel());
    }

    IEnumerator LeaveLevel()
    {
        Instantiate(canvasPrefab, transform);
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene(levelScene);
    }
}
