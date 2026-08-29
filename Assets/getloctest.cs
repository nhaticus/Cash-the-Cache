using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class getloctest : MonoBehaviour
{
    public loctest loctest;

    public LocalizeStringEvent mylocalstring;

    private void Start()
    {
        mylocalstring.StringReference = loctest.itemName;
    }
}
