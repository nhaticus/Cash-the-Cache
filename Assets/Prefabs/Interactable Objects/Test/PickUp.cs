using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// stealable object copy
public class PickUp : MonoBehaviour
{
    public ItemInfo itemToPickUp;

    public void SetItem(ItemInfo item)
    {
        itemToPickUp = item;
    }
}
