using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    public List<LootInfo> inventory = new List<LootInfo>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null)
        {
            if (objRef.GetComponent<StealableObject>())
            {
                inventory.Add(objRef.GetComponent<StealableObject>().lootInfo);
            }
            ExecuteEvents.Execute<InteractEvent>(objRef, null, (x, y) => x.Interact());
        }
        if (Input.GetMouseButtonDown(1) && inventory.Count > 0)
        {
            RevealInventory();
        }
    }
    private void OnTriggerEnter(Collider other) // CHECK: might be triggered multiple times if overlapping multiple things
    {
        objRef = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        objRef = null;
    }

    private void RevealInventory()
    {
        foreach (var loot in inventory)
        {
            Debug.Log(loot.name + ", val: " + loot.value + ", weight: " + loot.weight);
        }
    }
}
