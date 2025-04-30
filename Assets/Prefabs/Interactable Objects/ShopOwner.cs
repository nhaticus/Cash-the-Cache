using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopOwner : MonoBehaviour, InteractEvent
{
    [SerializeField] ShopManager shopManager;
    public void Interact()
    {
        //open shop menu
        shopManager.OpenShop();
    }
}
