using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Box_Screw : MonoBehaviour
{

    [HideInInspector] public float clicksRequired = 0;
    float clicks = 0;

    [HideInInspector] public UnityEvent removeScrew;
    public void Clicked()
    {
        clicks += PlayerManager.Instance.GetBoxOpening();
        transform.Rotate(new Vector3(0, 0, Random.Range(20, 40)));
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
