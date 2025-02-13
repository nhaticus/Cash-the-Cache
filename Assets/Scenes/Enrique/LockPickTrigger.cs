using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickTrigger : MonoBehaviour
{
    public GameObject lockpickingPanel;
    public GameObject player;
    private bool isNearSafe = false;

    // Start is called before the first frame update
    void Start()
    {
        lockpickingPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isNearSafe && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Lock Picking Started");
            OpenLockpicking();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player entered the safe area
        {
            Debug.Log("Player is in safe area");
            isNearSafe = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player left the safe area
        {
            Debug.Log("Player left safe area");
            isNearSafe = false;
        }
    }

    void OpenLockpicking()
    {
        PlayerManager.Instance.lockRotation();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        lockpickingPanel.SetActive(true);
  
       // player.GetComponent<FirstPersonController>().enabled = false; // Disable movement (if using FPS controller)
    }

}
