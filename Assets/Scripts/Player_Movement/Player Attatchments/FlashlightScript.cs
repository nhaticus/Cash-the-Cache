using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlashlightScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject flashlight;
    void Start()
    {
        if (!UpgradeManager.Instance.checkFlashlight())
        {
            flashlight.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && UpgradeManager.Instance.checkFlashlight())
        {
            Debug.Log("flashliht boom");
            flashlight.SetActive(!flashlight.activeSelf);
        }
    }

}
