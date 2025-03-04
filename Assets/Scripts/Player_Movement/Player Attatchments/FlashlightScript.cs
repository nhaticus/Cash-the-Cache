using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!UpgradeManager.Instance.checkFlashlight())
        {
            gameObject.SetActive(false);
        }
    }

}
