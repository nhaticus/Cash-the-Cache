using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    private Renderer objRenderer;
    private Color originalColor; // Store the original color of the object
    public Dictionary<string, (int, LootInfo)> inventory = new Dictionary<string, (int, LootInfo)>(); // Dictionary of item name as key, (number owned, Loot info)
    public int weight = 0;
    public int maxWeight = 30;

    [SerializeField] private float highlightIntensity = 1.5f; // How much lighter the object should get
    [SerializeField] float raycastDistance = 3.0f;
    [SerializeField] GameObject camera;

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

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, camera.transform.forward * raycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, camera.transform.forward, out hit, raycastDistance))
        {
            if (hit.transform.CompareTag("Selectable"))
            {
                if (objRef != hit.transform.gameObject) // Only update if a new object is hit
                {
                    //ResetHighlight(); // Reset previous object's color

                    objRef = hit.transform.gameObject;
                    objRenderer = objRef.GetComponent<Renderer>();

                    if (objRenderer != null)
                    {
                        originalColor = objRenderer.material.color; // Store original color
                        Color highlightedColor = originalColor * highlightIntensity; // Make it lighter
                        highlightedColor.a = originalColor.a; // Preserve transparency
                        objRenderer.material.color = highlightedColor; // Apply new color
                    }
                }
            }
            else
            {
                ResetHighlight();
            }
        }
        else
        {
            ResetHighlight();
        }
    }

    private void ResetHighlight()
    {
        if (objRef != null && objRenderer != null)
        {
            objRenderer.material.color = originalColor; // Restore original color
        }
        objRef = null;
        objRenderer = null;
    }

    [HideInInspector] public UnityEvent ItemTaken;
    private void Interact(GameObject obj)
    {
        StealableObject stealObj = obj.GetComponent<StealableObject>();
        if (stealObj != null)
        {
            if (weight <= maxWeight) // can steal over max weight once: but suffer more speed loss
            {
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
