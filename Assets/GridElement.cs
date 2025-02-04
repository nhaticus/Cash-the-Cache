using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GridElement : MonoBehaviour
{
    public Image img;
    public TMP_Text numOwned;

    public LootInfo lootInfo;

    /*
     * if clicked
     * send signal to inventory ui to change item info:
     * name, weight, and image
     */

    [HideInInspector] public UnityEvent<LootInfo> gridElementClicked;
    public void OnClick()
    {
        gridElementClicked.Invoke(lootInfo);
    }
}
