using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    public List<LootInfo> inventory = new List<LootInfo>();
    public int weight = 0;
    public int maxWeight = 30;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null)
        {
            Interact(objRef);
        }
        if (Input.GetMouseButtonDown(1) && inventory.Count > 0)
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
            objRef = hit.transform.gameObject;
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
                inventory.Add(stealObj.lootInfo);
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

    private void RevealInventory()
    {
        foreach (var loot in inventory)
        {
            Debug.Log(loot.name + ", val: " + loot.value + ", weight: " + loot.weight);
        }
    }
}
