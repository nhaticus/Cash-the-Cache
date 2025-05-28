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

    bool playerInLeaveArea = false;

    private void Start()
    {
        vanText.SetActive(false);
        warningText.SetActive(false);
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
            PlayerInteract playerInventory = FindObjectOfType<PlayerInteract>();

            if (VanInventory.Instance && playerInventory)
            {
                PlayerManager.Instance.ableToInteract = false; // stop movement
                PlayerManager.Instance.lockRotation();
            }
            else
            {
                Debug.LogError("VanInventory or PlayerInventory is NON-EXISTANT!");
            }
            ShowSummary();
        }
    }

    void ShowSummary()
    {
        // make player invincible
        PlayerHealth player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerHealth>();
        if (player)
            player.canHurt = false;
        singleAudio.PlaySFX("drive_away");
        resultScreen.SetActive(true);
        resultScreen.GetComponent<ResultScreen>().inventoryRef = VanInventory.Instance.stolenItems;
        resultScreen.GetComponent<ResultScreen>().Begin();
    }
}