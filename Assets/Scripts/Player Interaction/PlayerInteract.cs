using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    public Dictionary<string, (int, LootInfo)> inventory = new Dictionary<string, (int, LootInfo)>(); // Dictionary of item name as key, (number owned, Loot info)
    public int weight = 0;
    public int maxWeight = 30;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null)
        {
            Interact(objRef);
        }
        if (Input.GetMouseButtonDown(1))
        {
            RevealInventory();
        }
    }

    [SerializeField] float raycastDistance = 3.0f;
    [SerializeField] GameObject camera;
    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, camera.transform.forward * raycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, camera.transform.forward, out hit, raycastDistance))
        {
            if(hit.transform.tag == "Selectable")
            {
                objRef = hit.transform.gameObject;
            }
        }
        else
        {
            objRef = null;
        }
            
    }

    [HideInInspector] public UnityEvent ItemTaken;
    private void Interact(GameObject obj)
    {
        StealableObject stealObj = obj.GetComponent<StealableObject>();
        if (stealObj != null)
        {
            if (weight <= maxWeight) // can steal over max weight once: but suffer more speed loss
            {
                Debug.Log("pick up");
                if (inventory.ContainsKey(stealObj.lootInfo.itemName))
                {
                    inventory[stealObj.lootInfo.itemName] = (inventory[stealObj.lootInfo.itemName].Item1 + 1, stealObj.lootInfo);
                }
                else
                {
                    inventory.Add(stealObj.lootInfo.itemName, (1, stealObj.lootInfo));
                }

                weight += stealObj.lootInfo.weight;
                ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());

                ItemTaken.Invoke(); // send event saying an item was taken
            }
        }
        else
        {
            ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());
        }
    }

    [HideInInspector] public UnityEvent ShowInventory;
    private void RevealInventory()
    {
        ShowInventory.Invoke(); // send event saying to show inventory menu
    }
}
