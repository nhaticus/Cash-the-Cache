using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInteract : MonoBehaviour
{
    GameObject objRef = null;
    Renderer objRenderer;
    Material originalMaterial; // Store the original material of the object
    private Transform myTransform;

    [SerializeField] float raycastDistance = 2.8f;
    public GameObject mainCamera;

    [SerializeField] SingleAudio singleAudio;

    public Dictionary<string, (int, LootInfo)> inventory = new Dictionary<string, (int, LootInfo)>(); // Dictionary for inventory items
    public Tuple<(LootInfo, int)> newInventory; // type of item, number owned

    private void Awake()
    {
        myTransform = transform;
    }

    private void Update()
    {
        // Interact
        if (((UserInput.Instance && UserInput.Instance.Interact) || (UserInput.Instance == null && Input.GetMouseButtonDown(0)))
            && objRef != null && (PlayerManager.Instance == null || (PlayerManager.Instance != null && PlayerManager.Instance.ableToInteract))
            && Time.timeScale > 0)
        {
            Interact(objRef);
        }
    }

    Color origColor;
    void FixedUpdate()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.yellow);

        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * raycastDistance, raycastDistance);
        GameObject closestObj = null; float closestDist = 10;
        float wallDist = 10;
        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hit = hits[i].transform.gameObject;
            if (hit.CompareTag("Selectable"))
            {
                float distance = hits[i].distance;
                if (distance < closestDist)
                {
                    closestDist = distance;
                    closestObj = hit;
                }
            }
            if (hit.CompareTag("Wall")) // prevent raycasting through a wall
            {
                float distance = hits[i].distance;
                if (distance < wallDist)
                {
                    wallDist = distance;
                }
            }
        }

        if (closestObj == null)
        {
            ResetHighlight();
            ObjectSelected.Invoke(null);
        }
        else if (wallDist > closestDist && closestObj != null && closestObj != objRef)
        {
            SetSelectedObject(closestObj);
        }
    }

    [HideInInspector] public UnityEvent<GameObject> ObjectSelected;
    void SetSelectedObject(GameObject selectedObj)
    {
        ResetHighlight(); // Reset previous object's material

        objRef = selectedObj;
        ApplyColorToObject(objRef);

        ObjectSelected.Invoke(selectedObj);
    }

    void ApplyColorToObject(GameObject obj)
    {
        objRenderer = obj.GetComponent<Renderer>();
        originalMaterial = objRenderer.material;
        origColor = originalMaterial.color;

        if (originalMaterial != null)
        {
            Material newMat = objRenderer.material;
            if(obj.GetComponent<StealableObject>())
                newMat.color = Color.red;
            else
                newMat.color = Color.green;
            objRenderer.material = newMat; // Apply highlight material
        }
    }

    private void ResetHighlight()
    {
        if (objRef && objRenderer && originalMaterial)
        {
            objRenderer.material = originalMaterial; // Restore original material
            objRenderer.material.color = origColor;
        }
        objRef = null;
        objRenderer = null;
        originalMaterial = null;
    }

    
    [HideInInspector] public UnityEvent<bool> ItemTaken;
    /// <summary>
    /// Executes object's interact event.
    /// If the object is stealable, tries to put into inventory first.
    /// If not able to go into inventory: interact event fails.
    /// </summary>
    private void Interact(GameObject obj)
    {
        StealableObject stealObj = obj.GetComponent<StealableObject>();
        if (stealObj != null)
        {
            if (PlayerManager.Instance.getWeight() + stealObj.lootInfo.weight > PlayerManager.Instance.getMaxWeight()){
                singleAudio.PlaySFX("inventory_full");
            }

            if (PlayerManager.Instance.getWeight() + stealObj.lootInfo.weight <= PlayerManager.Instance.getMaxWeight())
            {
                singleAudio.PlaySFX("collect_item_sound");
                AddItemToInventory(stealObj.lootInfo);

                ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());

                PlayerManager.Instance.WeightChangeSpeed();
                ItemTaken.Invoke(true); // Send event saying an item was taken so weight UI changes

                if (TaskManager.Instance != null) TaskManager.Instance.task1Complete();
            }
            else
            {
                ItemTaken.Invoke(false); // too heavy, show weight UI jiggle
            }
        }
        else
        {
            // If it�s a door, pass the player�s Transform so it can swing away
            Doors door = obj.GetComponent<Doors>();
            if (door != null)
                door.Interact(myTransform);          // <<< new overload 
            else
                ExecuteEvents.Execute<InteractEvent>(obj, null, (x, y) => x.Interact());
        }
    }

    void AddItemToInventory(LootInfo info)
    {
        if (inventory.ContainsKey(info.itemName))
            inventory[info.itemName] = (inventory[info.itemName].Item1 + 1, info);
        else
            inventory.Add(info.itemName, (1, info));

        PlayerManager.Instance.addWeight(info.weight);
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

        Debug.Log("Player Speed set to: " + newSpeed.ToString());
        PlayerManager.Instance.setMoveSpeed(newSpeed);
    }
}
