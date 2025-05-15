// This script handles the collider where the player can leave the level. It displays a prompt to 
// the player when they enter the trigger area and handles the actions when they press 'E' to leave.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaveAreaTrigger : MonoBehaviour
{
    [SerializeField] GameObject vanText;
    // [SerializeField] GameObject vanTextForShader;
    [SerializeField] GameObject resultScreen;
    [SerializeField] SingleAudio singleAudio;

    bool playerInLeaveArea = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = true;
            vanText.SetActive(true);
            vanText.GetComponent<TMP_Text>().text = "Press E to leave";

            // vanTextForShader.SetActive(true);
            // vanTextForShader.GetComponent<TMP_Text>().text = "Press E to leave";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = false;
            vanText.SetActive(false);

            // vanTextForShader.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInLeaveArea && Input.GetKeyDown(KeyCode.E))
        {
            PlayerInteract playerInventory = FindObjectOfType<PlayerInteract>();

            if (VanInventory.Instance && playerInventory)
            {
                PlayerManager.Instance.ableToInteract = false; // stop movement
                PlayerManager.Instance.lockRotation();
            }
            else
            {
                Debug.LogError("VanInventory or PlayerInventory is NULL!");
            }
            ShowSummary();
        }
    }

    void ShowSummary()
    {
        // make player invincible or freeze game
        PlayerHealth player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerHealth>();
        if (player)
            player.canHurt = false;
        singleAudio.PlaySFX("drive_away");
        resultScreen.SetActive(true);
        resultScreen.GetComponent<ResultScreen>().inventoryRef = VanInventory.Instance.stolenItems;
        resultScreen.GetComponent<ResultScreen>().Begin();
    }
}