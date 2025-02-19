using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/*
 * Drop Item help (confusing)
 * https://www.youtube.com/watch?v=TSZmvF2oDCg&ab_channel=FeedMyKids
 */

public class InventoryUI : MonoBehaviour
{
    PlayerInteract playerInteract;
    
    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ChangeItemInfo(null);
    }

    public void Initialize(GameObject player)
    {
        playerInteract = player.GetComponent<PlayerInteract>();
        playerInteract.ShowInventory.AddListener(SwitchInventoryView);
    }

    bool show = false;
    public void SwitchInventoryView()
    {
        show = !show;
        if (show)
            ShowInventory();
        else
            HideInventory();
    }

    private void ShowInventory()
    {
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.slowPlayer();
        GetComponent<CanvasGroup>().alpha = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FillInventoryGrid();
    }

    private void HideInventory() // on exit button
    {
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unSlowPlayer();
        GetComponent<CanvasGroup>().alpha = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Get player's current inventory and fill grid with grid elements
    [SerializeField] Transform gridTransform;
    [SerializeField] GameObject gridElement;
    private void FillInventoryGrid()
    {
        // clear grid
        foreach (Transform child in gridTransform)
        {
            Destroy(child.gameObject);
        }

        // fill grid
        foreach (KeyValuePair<string, (int, LootInfo)> info in playerInteract.inventory)
        {
            GameObject newElement = Instantiate(gridElement, gridTransform);
            newElement.GetComponent<GridElement>().lootInfo = info.Value.Item2;
            newElement.GetComponent<GridElement>().img.sprite = info.Value.Item2.sprite;
            newElement.GetComponent<GridElement>().numOwned.text = info.Value.Item1.ToString();

            newElement.GetComponent<GridElement>().gridElementClicked.AddListener(ChangeItemInfo);
        }
    }

    [SerializeField] TMP_Text itemName, itemWeight;
    [SerializeField] Image itemImg;
    LootInfo selectedItem;
    private void ChangeItemInfo(LootInfo info)
    {
        selectedItem = info;
        if(selectedItem == null)
        {
            itemName.text = "none";
            itemWeight.text = "weight: 0";
            itemImg.sprite = null;
        }
        else
        {
            itemName.text = selectedItem.itemName;
            itemWeight.text = "weight: " + selectedItem.weight;
            itemImg.sprite = selectedItem.sprite;
        }
        
    }

    public void DropItem()
    {
        // create prefab
        Vector3 newObjectLocation = (playerInteract.camera.transform.forward * 2.5f) + playerInteract.transform.position + new Vector3(0, 1, 0);
        StealableObject o = Instantiate(selectedItem.Prefab.GetComponent<StealableObject>(), newObjectLocation, playerInteract.camera.transform.rotation);
        o.SetInfo(selectedItem);

        PlayerManager.Instance.subWeight(selectedItem.weight); //decrease weight
        playerInteract.WeightChangeSpeed(); // change player speed
        playerInteract.ItemTaken.Invoke(); // update weight UI

        // remove from inventory
        playerInteract.inventory[selectedItem.itemName] = (playerInteract.inventory[selectedItem.itemName].Item1 - 1, selectedItem);
        if(playerInteract.inventory[selectedItem.itemName].Item1 == 0)
        {
            playerInteract.inventory.Remove(selectedItem.itemName);
            ChangeItemInfo(null);
        }
        
        FillInventoryGrid(); // update grid
    }
}
