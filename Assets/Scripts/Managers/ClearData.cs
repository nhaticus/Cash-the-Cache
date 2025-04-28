using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearData : MonoBehaviour
{
    public void Clear()
    {
        int colorblind = PlayerPrefs.GetInt("ColorblindMode"); // keep colorblind
        PlayerPrefs.DeleteAll();
        GameManager.Instance.playerMoney = 0;
        PlayerPrefs.SetInt("ColorblindMode", colorblind); // restore colorblind
    }
}
