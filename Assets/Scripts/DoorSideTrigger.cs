using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorSideTrigger : MonoBehaviour
{
    public Doors doorScript;       // Assign in Inspector (the parent Doors script)
    public bool isFrontTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
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
        if (other.CompareTag("AI"))
        {
            if (isFrontTrigger)
                doorScript.OnFrontExit();
            else
                doorScript.OnBackExit();
        }
    }
}
