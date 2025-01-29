using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    public List<LootInfo> inventory = new List<LootInfo>();
    public int weight = 0;
    public int weightMax = 30;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null)
        {
            StealableObject stealObj = objRef.GetComponent<StealableObject>();
            if (stealObj != null)
            {
                if(weight <= weightMax) // you can steal over the weight max but cant steal when past it
                {
                    inventory.Add(stealObj.lootInfo);
                    weight += stealObj.lootInfo.weight;
                    ExecuteEvents.Execute<InteractEvent>(objRef, null, (x, y) => x.Interact());
                }
            }
            else
            {
                ExecuteEvents.Execute<InteractEvent>(objRef, null, (x, y) => x.Interact());
            }
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

    private void RevealInventory()
    {
        foreach (var loot in inventory)
        {
            Debug.Log(loot.name + ", val: " + loot.value + ", weight: " + loot.weight);
        }
    }
}
