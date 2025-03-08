using UnityEngine;

public class PoliceTimerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<PoliceTimer>().PauseTimer();
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
