using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Box_Screw : MonoBehaviour
{

    [HideInInspector] public int clicksRequired = 0;
    int clicks = 0;

    [HideInInspector] public UnityEvent removeScrew;
    public void Clicked()
    {
        clicks++;
        transform.Rotate(new Vector3(0, 0, Random.Range(20, 40)));
        Color c = GetComponent<Image>().color;
        c.a -= 1 * (1 / (clicksRequired * 1.3f));
        GetComponent<Image>().color = c;
        if (clicks == clicksRequired)
        {
            removeScrew.Invoke();
            Destroy(gameObject);
        }
    }

}
