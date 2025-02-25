using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef;
    Renderer objRenderer;
    Color originalColor; // Store the original color of the object
    [SerializeField] float highlightIntensity = 3f; // How much lighter the object should get

    public Dictionary<string, (int, LootInfo)> inventory = new Dictionary<string, (int, LootInfo)>(); // Dictionary of item name as key, (number owned, Loot info)

    [SerializeField] float raycastDistance = 3.0f;
    public GameObject camera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null && PlayerManager.Instance.ableToInteract)
        {
            // If any lockpicking is open, skip
            if (LockPicking.anyLockpickingOpen)
                return;

            LockPicking safeLock = objRef.GetComponent<LockPicking>();
            if (safeLock != null && !safeLock.isUnlocked)
            {
                safeLock.OpenLockpicking();  // Single-script approach
            }
            else
            {
                // Normal Interact
                Interact(objRef);
            }
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
            // Check if it's a safe with LockPicking script
            LockPicking safeLock = hit.transform.GetComponent<LockPicking>();
            if (safeLock != null)
            {
                // Highlight if not already highlighting this object
                if (objRef != safeLock.gameObject)
                {
                    ResetHighlight();
                    objRef = safeLock.gameObject;
                    objRenderer = objRef.GetComponent<Renderer>();
                    if (objRenderer != null)
                    {
                        originalColor = objRenderer.material.color;
                        Color highlightedColor = originalColor * highlightIntensity;
                        highlightedColor.a = originalColor.a;
                        objRenderer.material.color = highlightedColor;
                    }
                }

                // If player left-clicks on the safe, try to open lockpicking
                if (Input.GetMouseButtonDown(0) && PlayerManager.Instance.ableToInteract)
                {
                    safeLock.OpenLockpicking();
                }
            }
            else
            {
                // If it's not a safe, maybe it's a normal "Selectable" or StealableObject
                if (hit.transform.CompareTag("Selectable"))
                {
                    // The old highlight logic
                    if (objRef != hit.transform.gameObject)
                    {
                        ResetHighlight();
                        objRef = hit.transform.gameObject;
                        objRenderer = objRef.GetComponent<Renderer>();
                        if (objRenderer != null)
                        {
                            originalColor = objRenderer.material.color;
                            Color highlightedColor = originalColor * highlightIntensity;
                            highlightedColor.a = originalColor.a;
                            objRenderer.material.color = highlightedColor;
                        }
                    }
                }
                else
                {
                    ResetHighlight();
                }
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

                ItemTaken.Invoke(); // send event saying an item was taken
            }
        }
        else
        {
            ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());
        }
    }

    public void WeightChangeSpeed()
    {
        float ChangeSpeedByPercent(float percent){ return PlayerManager.Instance.getMaxMoveSpeed() - (PlayerManager.Instance.getMaxMoveSpeed() * percent / 100);}

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
        ShowInventory.Invoke(); // send event saying to show inventory menu
    }
}
