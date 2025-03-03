using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A box object that when clicked on creates a minigame
 * The minigame spawns screws on the screw which need to be clicked to remove
 * When all screws are removed a game object is spawned and the box is destroyed
 */

public class Box : MonoBehaviour, InteractEvent
{
    [SerializeField] GameObject[] obj;
    [SerializeField] int difficulty = 4; // difficulty determines amount of clicks (difficulty * 1.5) and number of screws

    [SerializeField] GameObject boxCanvas;
    [SerializeField] GameObject screw;
    int screwsLeft = 0;

    private void Start()
    {
        screwsLeft = difficulty;
    }

    // Create difficulty amount of screws and connect their event to ScrewOff
    public void Interact()
    {
        GameObject canvas = Instantiate(boxCanvas, transform);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);


        for (int i = 0; i < screwsLeft; i++)
        {
            GameObject screwObj = Instantiate(screw);
            screwObj.transform.SetParent(canvas.transform);
            screwObj.transform.localPosition = new Vector3(Random.Range(-620, 620), Random.Range(-320, 320), 0);
            Box_Screw screwScript = screwObj.GetComponent<Box_Screw>();
            screwScript.clicksRequired = Mathf.RoundToInt(difficulty * 1.5f);
            screwScript.removeScrew.AddListener(ScrewOff);
        }
    }

    void ScrewOff() // when a screw comes off this event is used
    {
        screwsLeft--;
        if(screwsLeft == 0)
            OpenBox();
    }

    void OpenBox()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        Instantiate(obj[Random.Range(0, obj.Length - 1)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
