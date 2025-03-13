using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearData : MonoBehaviour
{
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
        UpgradeManager.Instance.ResetData();
        GameManager.Instance.numRuns = 0;
    }
}
