using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Doors : MonoBehaviour, InteractEvent
{
    public NavMeshObstacle obstacle;
    [Header("Door Mesh/Pivot")]
    public Transform door;            // The door mesh or pivot

    [Header("Angles & Speed")]
    public float frontOpenAngle = 80f;  // Angle for opening from front
    public float backOpenAngle = -80f; // Angle for opening from back
    public float closeAngle = 0f;   // Angle when closed
    public float speed = 3f;   // Rotation speed

    [Header("Audio Settings")]
    public AudioClip doorCloseClip;     // Assign the door closing sound in the Inspector
    private AudioSource doorAudioSource;  // Local reference to AudioSource

    private bool isOpen = false;
    private bool lastSideFront = true;

    private int frontCount = 0;    // How many NPCs are in the front trigger
    private int backCount = 0;    // How many NPCs are in the back trigger

    private void Awake()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.carveOnlyStationary = false;
        obstacle.enabled = isOpen;
        obstacle.carving = isOpen;

        doorAudioSource = GetComponent<AudioSource>();
        if (doorAudioSource == null)
        {
            doorAudioSource = gameObject.AddComponent<AudioSource>();
        }
        doorAudioSource.spatialBlend = 1f; // Set to 3D sound
        doorAudioSource.maxDistance = 10f;   // Lower max distance
        doorAudioSource.rolloffMode = AudioRolloffMode.Linear; 
    }
    // ----------------------------------------------------------------------
    // Methods Called By DoorSideTrigger
    // ----------------------------------------------------------------------

    public void OnFrontEnter()
    {
        frontCount++;
        lastSideFront = true;

        if (!isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(OpenDoor());
        }
    }

    public void OnFrontExit()
    {
        frontCount = Mathf.Max(frontCount - 1, 0);

        if ((frontCount + backCount) <= 0 && isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(CloseDoor());
        }
    }

    public void OnBackEnter()
    {
        backCount++;
        lastSideFront = false;

        if (!isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(OpenDoor());
        }
    }

    public void OnBackExit()
    {
        backCount = Mathf.Max(backCount - 1, 0);

        if ((frontCount + backCount) <= 0 && isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(CloseDoor());
        }
    }

    // ----------------------------------------------------------------------
    // Player Interaction
    // ----------------------------------------------------------------------

    public void Interact()
    {
        // If the door is open, close it; if closed, open it
        if (isOpen)
        {
            StopAllCoroutines();
            StartCoroutine(CloseDoor());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(OpenDoor());
        }
    }

    // ----------------------------------------------------------------------
    // Coroutines
    // ----------------------------------------------------------------------

    private IEnumerator OpenDoor()
    {
        isOpen = true;
        float currentAngle = door.localEulerAngles.y;
        float targetAngle = lastSideFront ? frontOpenAngle : backOpenAngle;
        obstacle.enabled = true;
        obstacle.carving = true;

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * speed);
            door.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
    }

    private IEnumerator CloseDoor()
    {
        isOpen = false;
        float currentAngle = door.localEulerAngles.y;
        obstacle.enabled = false;
        obstacle.carving = false;

            doorAudioSource.PlayOneShot(doorCloseClip);

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, closeAngle)) > 0.1f)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, closeAngle, Time.deltaTime * speed);
            door.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
    }
}
