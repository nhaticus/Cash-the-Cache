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
    [SerializeField] private float highlightIntensity = 1.5f; // How much lighter the object should get

    public Dictionary<string, (int, LootInfo)> inventory = new Dictionary<string, (int, LootInfo)>(); // Dictionary of item name as key, (number owned, Loot info)

    [SerializeField] float raycastDistance = 3.0f;
    public GameObject camera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && objRef != null && PlayerManager.Instance.ableToInteract)
        {
            Interact(objRef);
        }
        if (Input.GetMouseButtonDown(1) && PlayerManager.Instance.ableToInteract && !FindObjectOfType<LockPicking>().isLockpicking)
        {
            RevealInventory();
        }
    }

    void FixedUpdate()
    {
        // Get the screen center point
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // Generate a ray from the camera through the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance))
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
