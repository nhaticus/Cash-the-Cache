using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Collider for player to deposit items
 */

public class VanTrigger : MonoBehaviour
{
    bool playerInRange = false;
    PlayerInteract playerInventory;
    [SerializeField] GameObject vanText;


    private float depositTimer = 0f;
    private bool depositCompleted = false;
    private float baseLoadingTime = 1.0f;    // Base time required to deposit
    private float extraTimePerItem = 0.5f;     // Additional time required per item

    private void Awake()
    {
        vanText.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInteract>();

            vanText.SetActive(true);
            vanText.GetComponent<TMP_Text>().text = "Press E to Deposit";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;

            vanText.SetActive(false);
            depositTimer = 0f;
            depositCompleted = false;
        }
    }

    private void Update()
    {
        if (playerInRange && playerInventory != null) 
        {
            if (Input.GetKey(KeyCode.E)) 
            {
                //Calc required hold time based on player's item count
                int itemCount = playerInventory.inventory.Count;
                if (itemCount == 0)
                {
                    // No items -> Skip deposit logic and show message
                    depositTimer = 0f;
                    depositCompleted = false;
                    vanText.GetComponent<TMP_Text>().text = "No items to deposit!";
                    return; 
                }

                float requiredHoldTime = baseLoadingTime + (itemCount * extraTimePerItem);

                depositTimer += Time.deltaTime;

                int progressPercent = Mathf.Clamp((int)((depositTimer / requiredHoldTime) * 100), 0, 100);
                vanText.GetComponent<TMP_Text>().text = "Depositing... " + progressPercent + "%";

                if (depositTimer >= requiredHoldTime && !depositCompleted)
                {
                    //Checks if task Manager is present then checks off task 2
                    if (TaskManager.Instance != null)
                    {
                        TaskManager.Instance.task2Complete();
                    }
                    DepositItems();
                    depositCompleted = true;
                }
            }
            else
            {
                // If the player releases E, reset the deposit process
                depositTimer = 0f;
                depositCompleted = false;
                vanText.GetComponent<TMP_Text>().text = "Hold E to Deposit";
            }
        }


    }

    private void DepositItems()
    {
        if (VanInventory.Instance != null && playerInventory != null)
        {
            VanInventory.Instance.TransferItemsFromPlayer(playerInventory);
            PlayerManager.Instance.setWeight(0);
            vanText.GetComponent<TMP_Text>().text = "Items Deposited!";
        }
        else
        {
            Debug.LogError("VanInventory or PlayerInventory is NULL!");
        }
    }
}