using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    PlayerInteract playerInteract;
    public void Initialize(GameObject player)
    {
        playerInteract = player.GetComponent<PlayerInteract>();
        playerInteract.ShowInventory.AddListener(ShowInventory);
    }

    private void ShowInventory()
    {
        PlayerManager.Instance.lockRotation();
        GetComponent<CanvasGroup>().alpha = 1;
    }

    public void HideInventory() // on exit button
    {
        PlayerManager.Instance.unlockRotation();
        GetComponent<CanvasGroup>().alpha = 0;
    }
}
