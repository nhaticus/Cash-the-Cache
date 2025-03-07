using UnityEngine;

public class PoliceTimerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<PoliceTimer>().StopTimerFor3Minutes();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<PoliceTimer>().ResumeTimer();
        }
    }
}
