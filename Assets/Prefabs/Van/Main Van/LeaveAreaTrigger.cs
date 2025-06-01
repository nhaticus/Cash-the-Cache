// This script handles the collider where the player can leave the level. It displays a prompt to 
// the player when they enter the trigger area and handles the actions when they press 'E' to leave.
using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class LeaveAreaTrigger : MonoBehaviour
{
    [SerializeField] GameObject vanText, warningText;
    [SerializeField] GameObject resultScreen;
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] LocalizedString pressToLeaveString;

    VanInventory vanInventory;

    bool playerInLeaveArea = false;

    private void Start()
    {
        vanText.SetActive(false);
        warningText.SetActive(false);
        vanInventory = transform.parent.GetComponent<VanInventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = true;
            vanText.SetActive(true);
            pressToLeaveString.GetLocalizedStringAsync().Completed += handle => {
                vanText.GetComponent<TMP_Text>().text = handle.Result;
            };

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        { // check if items left in inventory
            warningText.SetActive(other.GetComponent<PlayerInteract>().inventory.Count > 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLeaveArea = false;
            warningText.SetActive(false);
            vanText.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInLeaveArea && Input.GetKeyDown(KeyCode.E))
        {
            // Check for player's inventory and create result screen
            PlayerInteract playerInventory = FindObjectOfType<PlayerInteract>();

            if (vanInventory && playerInventory)
            {
                PlayerManager.Instance.ableToInteract = false; // stop movement
                PlayerManager.Instance.lockRotation();
            }
            else
            {
                Debug.LogError("VanInventory or PlayerInventory is NON-EXISTANT!");
            }
            ShowResultScreen();
        }
    }

    void ShowResultScreen()
    {
        // make player invincible
        PlayerHealth player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerHealth>();
        if (player)
            player.canHurt = false;

        singleAudio.PlaySFX("drive_away");

        // create and populate result screen
        resultScreen.SetActive(true);
        resultScreen.GetComponent<ResultScreen>().inventoryRef = vanInventory.stolenItems;
        resultScreen.GetComponent<ResultScreen>().Begin();

        // set flashlight to off
        Item flashlight = DataSystem.GetOrCreateItem("Flashlight");
        flashlight.level = 0;
        DataSystem.SaveItems();
    }
}