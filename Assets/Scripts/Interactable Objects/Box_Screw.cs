using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Box_Screw : MonoBehaviour
{
    [HideInInspector] public int clicksRequired = 0;
    int clicks = 0;

    [HideInInspector] public UnityEvent removeScrew;

    public void Clicked()
    {
        clicks++;
        if(clicks == clicksRequired)
        {
            removeScrew.Invoke();
            Destroy(gameObject);
        }
    }

}
