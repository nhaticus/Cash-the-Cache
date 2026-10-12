using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Box_Screw : MonoBehaviour
{
    public float clicksRequired = 4;
    [HideInInspector] public float clickStrength = 1;
    float clicks = 0;

    [HideInInspector] public SingleAudio singleAudio; // given by box canvas

    [HideInInspector] public UnityEvent removeScrew;


    public void Clicked()
    {
        // increase clicks
        clicks += clickStrength;

        // play sound
        singleAudio.PlaySFX("unscrew");

        // rotate
        transform.Rotate(new Vector3(0, 0, Random.Range(20, 40)));

        // change opacity
        Color c = GetComponent<Image>().color;
        c.a = ((clicksRequired - clicks) * 1.2f) / clicksRequired;
        GetComponent<Image>().color = c;
        if (clicks >= clicksRequired)
        {
            removeScrew.Invoke();
            Destroy(gameObject);
        }
    }

}
