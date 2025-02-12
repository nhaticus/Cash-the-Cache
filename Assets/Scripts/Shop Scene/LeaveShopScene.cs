using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Script attached to leave area in shop scene
 * When player touches leave area they go back to main level
 */

public class LeaveShopScene : MonoBehaviour
{
    [SerializeField] string levelScene;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(levelScene);
    }
}
