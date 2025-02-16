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
    [SerializeField] int difficulty = 3; // difficulty determines amount of clicks (difficulty * 1.3) and number of screws

    [SerializeField] GameObject canvas, background;
    [SerializeField] GameObject screw;
    int screwsLeft = 0;
    bool interacted = false;

    private void Start()
    {
        canvas.SetActive(false);
        screwsLeft = difficulty;
    }
    public void Interact()
    {
        if (!interacted)
        {
            canvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            for (int i = 0; i < screwsLeft; i++)
            {
                // make sure not to spawn too close to each other
                Vector3 randPos = new Vector3(Random.Range(-500, 500), Random.Range(-300, 300), 0);
                GameObject screwObj = Instantiate(screw, randPos, Quaternion.identity);
                screwObj.transform.SetParent(background.transform);
                Box_Screw screwScript = screwObj.GetComponent<Box_Screw>();
                screwScript.clicksRequired = Mathf.RoundToInt(difficulty * 1.3f);
                screwScript.removeScrew.AddListener(ScrewOff);
            }
            interacted = true; // prevent opening when in canvas (NEED TO ASK AJ)
        }
    }

    void ScrewOff()
    {
        screwsLeft--;
        if(screwsLeft == 0)
        {
            OpenBox();
        }
    }

    void OpenBox()
    {
        canvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Instantiate(obj[Random.Range(0, obj.Length - 1)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
