using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * Set the OnPurchase function on a Shop Upgrade
 */
public interface UpgradeEvent : IEventSystemHandler
{
    void OnPurchase();
}
