using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxCanvas : MonoBehaviour
{
    [SerializeField] GameObject screw;
    [SerializeField] int minScrews = 3;
    int screwsLeft = 0;

    public SingleAudio singleAudio;

    public int difficulty;
    [SerializeField] float screwdriverStrength = 0.35f;
    [HideInInspector] public float strength;

    void Start()
    {
        // get box open strength
        Item screwdriver = DataSystem.GetItem("Screwdriver");
        strength = 1 + screwdriver.level * screwdriverStrength;

        // create screws
        screwsLeft = Mathf.FloorToInt(minScrews + (difficulty * 1.5f));

        int extraClicksRequired = 0;
        // increase pins based on difficulty
        if (1 < difficulty && difficulty <= 3) // 2-3 = +1 click
        {
            extraClicksRequired = 1;
        }
        else if (3 < difficulty) // 4-5 = +2 click
        {
            extraClicksRequired = 2;
        }
        else if (5 < difficulty) // 5-inf = +2 click
        {
            extraClicksRequired = 3;
        }

        for (int i = 0; i < screwsLeft; i++)
        {
            GameObject screwObj = Instantiate(screw);
            screwObj.transform.SetParent(transform);
            screwObj.transform.localPosition = new Vector3(Random.Range(-620, 620), Random.Range(-320, 320), 0);

            Box_Screw screwScript = screwObj.GetComponent<Box_Screw>();
            screwScript.singleAudio = singleAudio;
            screwScript.clicksRequired += extraClicksRequired;
            screwScript.removeScrew.AddListener(ScrewOff);
            screwScript.clickStrength = strength;
        }
    }

    [HideInInspector] public UnityEvent OpenBox;
    void ScrewOff() // when a screw comes off this event is used
    {
        screwsLeft--;
        if (screwsLeft == 0)
            OpenBox.Invoke();
    }

    private void Update()
    {
        if ((UserInput.Instance && UserInput.Instance.Cancel) ||
            (UserInput.Instance == null && Input.GetKeyDown(KeyCode.Escape)))
        {
            ExitBox();
        }
    }

    public void ExitBox()
    {
        // reset cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

        // reset player
        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject);
    }
}
