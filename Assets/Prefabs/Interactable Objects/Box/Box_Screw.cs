using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Box_Screw : MonoBehaviour
{

    [HideInInspector] public float clicksRequired = 0;
    float clicks = 0;

    SingleAudio singleAudio;

    [HideInInspector] public UnityEvent removeScrew;

    private void Start()
    {
        // get access to parent single Audio
        singleAudio = transform.parent.GetComponent<SingleAudio>();
    }

    public void Clicked()
    {
        // increase clicks
        clicks += PlayerManager.Instance.GetBoxOpening();

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
