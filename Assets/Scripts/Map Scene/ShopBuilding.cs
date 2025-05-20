using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopBuilding : BaseMapBuilding
{
    [Header("Map Dependencies")]
    [SerializeField] string levelScene;
    public override void CreateCanvas(GameObject playerCam)
    {
        CreateFollowingCanvas(playerCam);
    }

    public override void Interact()
    {
        SceneManager.LoadScene(levelScene);
    }
}
