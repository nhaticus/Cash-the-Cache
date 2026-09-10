using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class loctest : MonoBehaviour
{
    public LocalizedString itemName;

    private void Start()
    {
        Debug.Log(itemName.GetLocalizedString());
    }
}
