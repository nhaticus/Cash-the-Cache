using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    PlayerInteract playerInteract;
    

    private void Start()
    {
        HideInventory();
    }

    public void Initialize(GameObject player)
    {
        playerInteract = player.GetComponent<PlayerInteract>();
        playerInteract.ShowInventory.AddListener(ShowInventory);
    }

    private void ShowInventory()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        FillInventoryGrid();
    }

    public void HideInventory() // on exit button
    {
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

        itemName.text = selectedItem.itemName;
        itemWeight.text = "weight: " + selectedItem.weight;
        itemImg.sprite = selectedItem.sprite;
    }

    public void DropItem()
    {
        // create object (if prefab is attached create, otherwise create basic cube)
        Instantiate(selectedItem != null ? selectedItem.prefab : GameObject.CreatePrimitive(PrimitiveType.Cube), playerInteract.transform.position + transform.TransformDirection(new Vector3(0, 0, 2)), playerInteract.transform.rotation);

        // remove from inventory
        playerInteract.inventory[selectedItem.itemName] = (playerInteract.inventory[selectedItem.itemName].Item1 - 1, selectedItem);
        if(playerInteract.inventory[selectedItem.itemName].Item1 == 0)
        {
            playerInteract.inventory.Remove(selectedItem.itemName);
            selectedItem = new LootInfo();
            ChangeItemInfo(selectedItem);
        }

        playerInteract.weight -= selectedItem.weight; // decrease weight

        FillInventoryGrid(); // update grid
    }
}
