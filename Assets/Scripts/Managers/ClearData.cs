using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearData : MonoBehaviour
{
    public void Clear()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.playerMoney = 0;
        // need to reset shop prices which honestly should be taking from a player pref
    }
}
