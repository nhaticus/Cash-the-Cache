using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HitMarker : MonoBehaviour
{
    public static HitMarker Instance { get; private set; }

    [Header("Colors")]
    public Color hitColor = Color.white;
    public Color knockColor = Color.red;

    [Header("Timing")]
    public float displayTime = 0.2f;

    private Image[] allImages;

    void Awake()
    {
        // enforce singleton
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // grab every Image in this GameObject's children
        allImages = GetComponentsInChildren<Image>();

        // hide them at start
        SetAlpha(0);
    }

    public void Initialize(GameObject player)
    {
        // add player's hitbox listener to show hit effect
        player.GetComponentInChildren<Hitbox>().hitEvent.AddListener(ShowHit);
    }

    //Flash the marker white.
    public void ShowHit() => Show(hitColor);

    // Flash the marker red.
    public void ShowKnock() => Show(knockColor);

    private void Show(Color col)
    {
        StopAllCoroutines();

        // set each image to the chosen color + full alpha
        foreach (var img in allImages)
            img.color = new Color(col.r, col.g, col.b, 1f);

        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        yield return new WaitForSeconds(displayTime);
        SetAlpha(0);
    }

    /// <summary>
    /// Set alpha of all image components
    /// </summary>
    private void SetAlpha(float a)
    {
        foreach (var img in allImages)
        {
            var c = img.color;
            c.a = a;
            img.color = c;
        }
    }
}
