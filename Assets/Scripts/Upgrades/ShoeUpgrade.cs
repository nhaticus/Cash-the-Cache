using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeUpgrade : MonoBehaviour, UpgradeEvent
{
    public UpgradeItem upgradeItem;

    public void OnPurchase()
    {
        Debug.Log("Shoe bought");
    }
}
