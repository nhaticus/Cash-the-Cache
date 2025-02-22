using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public Transform door; // The door object to rotate
    public float openAngle = 80f;
    public float closeAngle = 0f;
    public float speed = 3f;
    private bool isOpen = false;
    private int objectsInTrigger = 0; // Track multiple AI

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            Debug.Log("Ai Enter");
            objectsInTrigger++;
            if (!isOpen)
            {
                StopAllCoroutines();
                StartCoroutine(OpenDoor());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            Debug.Log("Ai Closed");
            objectsInTrigger--;
            if (objectsInTrigger <= 0) // Only close if no one is inside
            {
                StopAllCoroutines();
                StartCoroutine(CloseDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isOpen = true;
        float currentAngle = door.localRotation.eulerAngles.y;
        float targetAngle = openAngle;

        while (Mathf.Abs(currentAngle - targetAngle) > 0.1f)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speed);
            door.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
    }

    IEnumerator CloseDoor()
    {
        isOpen = false;
        float currentAngle = door.localRotation.eulerAngles.y;
        float targetAngle = closeAngle;

        while (Mathf.Abs(currentAngle - targetAngle) > 0.1f)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speed);
            door.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
    }
}
