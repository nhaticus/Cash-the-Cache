using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxCanvas : MonoBehaviour
{
    [SerializeField] GameObject screw;
    int screwsLeft = 0;

    public int difficulty;
    void Start()
    {
        screwsLeft = difficulty;
        for (int i = 0; i < screwsLeft; i++)
        {
            GameObject screwObj = Instantiate(screw);
            screwObj.transform.SetParent(transform);
            screwObj.transform.localPosition = new Vector3(Random.Range(-620, 620), Random.Range(-320, 320), 0);
            Box_Screw screwScript = screwObj.GetComponent<Box_Screw>();
            screwScript.clicksRequired = Mathf.RoundToInt(difficulty * 1.7f);
            screwScript.removeScrew.AddListener(ScrewOff);
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
        if (UserInput.Instance && (UserInput.Instance.Cancel || UserInput.Instance.Pause))
        {
            ExitBox();
        }
    }

    public void ExitBox()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject);
    }
}
