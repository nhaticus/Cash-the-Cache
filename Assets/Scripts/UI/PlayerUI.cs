using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * UI that follows the first person player view
 * 
 * keeps searching for player and once found gives following items a reference to player:
 * items:
 *  weight
 *  player inventory
 */

public class PlayerUI : MonoBehaviour
{
    
    WeightUI weightUI;
    [SerializeField] GameObject inventoryPrefab;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    GameObject player;
    IEnumerator FindPlayer()
    {
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }

        weightUI = GetComponentInChildren<WeightUI>();
        weightUI.Initialize(player);
    }

    bool inventoryOpen = false; // check for if inventory or lock picking is open
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !inventoryOpen && (PlayerManager.Instance == null ||
            (PlayerManager.Instance != null && PlayerManager.Instance.ableToInteract))
            && Time.timeScale > 0)
        {
            CreateInventory();
        }
    }

    void CreateInventory()
    {
        inventoryOpen = true;
        GameObject inventory = Instantiate(inventoryPrefab, transform);
        inventory.GetComponent<InventoryUI>().Initialize(player);
        inventory.GetComponent<InventoryUI>().HideInventory.AddListener(HideInventory);
    }

    void HideInventory()
    {
        inventoryOpen = false;
    }

}
