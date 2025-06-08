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
    [SerializeField] WeightIndicator weightIndicator;

    [SerializeField] GameObject inventoryPrefab;

    [SerializeField] HitMarker hitMarker;

    [SerializeField] float checkForPlayerRate = 0.5f;

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
            yield return new WaitForSeconds(checkForPlayerRate);
        }

        weightUI = GetComponentInChildren<WeightUI>();
        weightUI.Initialize(player);

        weightIndicator.Initialize(player);

        hitMarker.Initialize(player);

        player.GetComponent<HealthController>().OnDeath.AddListener(HidePlayerUI);
    }

    bool inventoryOpen = false; // check for if inventory or lock picking is open
    private void Update()
    {
        // Open inventory if:
        // pressed inventory button, can interact, and game not paused
        if ((UserInput.Instance && UserInput.Instance.Inventory) || (UserInput.Instance == null && Input.GetKeyDown(KeyCode.I))
            && !inventoryOpen && (PlayerManager.Instance == null ||
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

    void HidePlayerUI()
    {
        gameObject.SetActive(false);
    }

}
