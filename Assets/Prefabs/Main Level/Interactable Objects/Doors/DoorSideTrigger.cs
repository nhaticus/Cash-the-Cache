//This script handles the detection of AI entering and exiting the door's trigger area on either the front or back of the door.
//It communicates with the parent Doors script to open or close the door based on the side the AI is on.
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorSideTrigger : MonoBehaviour
{
    public Doors doorScript;       // Assign in Inspector (the parent Doors script)
    public bool isFrontTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Trigger Enter: " + other.name);
        if (other.CompareTag("AI"))
        {
            if (isFrontTrigger)
                doorScript.OnFrontEnter();
            else
                doorScript.OnBackEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("Trigger Exit: " + other.name);
        if (other.CompareTag("AI"))
        {
            if (isFrontTrigger)
                doorScript.OnFrontExit();
            else
                doorScript.OnBackExit();
        }
    }
}
