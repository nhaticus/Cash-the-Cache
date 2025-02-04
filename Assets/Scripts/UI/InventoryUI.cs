using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private void ChangeItemInfo(LootInfo info)
    {
        itemName.text = info.name;
        itemWeight.text = "weight: " + info.weight;
        itemImg.sprite = info.sprite;
    }
}
