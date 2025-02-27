using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    private GameObject objRef;
    private Renderer objRenderer;
    private Material originalMaterial; // Store the original material of the object
    [SerializeField] private Material highlightMaterial; // Material to highlight object

    [SerializeField] private float raycastDistance = 3.0f;
    public GameObject camera;

    public Dictionary<string, (int, LootInfo)> inventory = new Dictionary<string, (int, LootInfo)>(); // Dictionary for inventory items

    private void Update()
    {
        // Left-click
        if (Input.GetMouseButtonDown(0) && objRef != null &&
            (PlayerManager.Instance == null || (PlayerManager.Instance != null && PlayerManager.Instance.ableToInteract)))
        {
            // If any lockpicking is open, skip normal logic
            if (LockPicking.anyLockpickingOpen)
                return;

            // Check if the object has LockPicking
            LockPicking safeLock = objRef.GetComponent<LockPicking>();
            if (safeLock != null && !safeLock.isUnlocked)
            {
                // It's a safe, open lockpicking
                safeLock.OpenLockpicking();
            }
            else
            {
                // Normal Interact
                //Checks if there is an existing task list
                if(TaskManager.Instance != null)
                {
                    TaskManager.Instance.task1Complete();
                }
                Interact(objRef);
            }
        }

        // Right-click: open inventory if not lockpicking
        if (Input.GetMouseButtonDown(1) && (PlayerManager.Instance == null ||
            (PlayerManager.Instance != null && PlayerManager.Instance.ableToInteract)) && !LockPicking.anyLockpickingOpen)
        {
            RevealInventory();
        }
    }

    void FixedUpdate()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.transform.CompareTag("Selectable"))
            {
                if (objRef != hit.transform.gameObject) // Only update if a new object is hit
                {
                    ResetHighlight(); // Reset previous object's material

                    objRef = hit.transform.gameObject;
                    objRenderer = objRef.GetComponent<Renderer>();

                    if (objRenderer != null)
                    {
                        originalMaterial = objRenderer.material; // Store original material
                        objRenderer.material = highlightMaterial; // Apply highlight material
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
        if (objRef != null && objRenderer != null && originalMaterial != null)
        {
            objRenderer.material = originalMaterial; // Restore original material
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
            if (PlayerManager.Instance.getWeight() + stealObj.lootInfo.weight <= PlayerManager.Instance.getMaxWeight())
            {
                AudioManager.Instance.PlaySFX("collect_item_sound");
                if (inventory.ContainsKey(stealObj.lootInfo.itemName))
                {
                    inventory[stealObj.lootInfo.itemName] = (inventory[stealObj.lootInfo.itemName].Item1 + 1, stealObj.lootInfo);
                }
                else
                {
                    inventory.Add(stealObj.lootInfo.itemName, (1, stealObj.lootInfo));
                }

                PlayerManager.Instance.addWeight(stealObj.lootInfo.weight);
                ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());

                WeightChangeSpeed();
                ItemTaken.Invoke(); // Send event saying an item was taken
            }
        }
        else
        {
            ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());
        }
    }

    public void WeightChangeSpeed()
    {
        float ChangeSpeedByPercent(float percent) 
        { 
            return PlayerManager.Instance.getMaxMoveSpeed() - (PlayerManager.Instance.getMaxMoveSpeed() * percent / 100);
        }

        float weightPercentage = (float) PlayerManager.Instance.getWeight() / PlayerManager.Instance.getMaxWeight();
        float newSpeed = PlayerManager.Instance.getMaxMoveSpeed();
        if (weightPercentage >= 0.9)
            newSpeed = ChangeSpeedByPercent(35); // 35% slower
        else if (weightPercentage > 0.8)
            newSpeed = ChangeSpeedByPercent(20); // 20% slower
        else if (weightPercentage > 0.6)
            newSpeed = ChangeSpeedByPercent(10); // 10% slower

        PlayerManager.Instance.setMoveSpeed(newSpeed);
    }

    [HideInInspector] public UnityEvent ShowInventory;
    private void RevealInventory()
    {
        ShowInventory.Invoke(); // Send event to show inventory menu
    }
}
