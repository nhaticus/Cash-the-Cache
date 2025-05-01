// This script manages the behavior of the door, including opening and closing it
// based on interactions from NPCs or players. It uses coroutines to smoothly rotate
// the door between open and closed states and handles obstacle carving for navigation.
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


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
    [SerializeField] SingleAudio singleAudio;

    private bool isOpen = false;
    private bool lastSideFront = true;

    private int frontCount = 0;    // How many NPCs are in the front trigger
    private int backCount = 0;    // How many NPCs are in the back trigger


    private static Transform player;

    private void Awake()
    {
        if (player == null)            // first door to awake does the lookup
            player = GameObject.FindWithTag("Player")?.transform;

        obstacle = GetComponent<NavMeshObstacle>();
        obstacle.carveOnlyStationary = false;
        obstacle.enabled = isOpen;
        obstacle.carving = isOpen;

        AudioSource doorAudioSource = singleAudio.sfxSource;
        if (doorAudioSource == null)
            doorAudioSource = gameObject.AddComponent<AudioSource>();
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
    //Inteface required wrapper
    public void Interact()
    {
        if (player == null)
        {
            Debug.Log("Doors.cs - Player == NULL in Interact()");
            return;
        } 
        Interact(player);                 // ➜ calls the real logic below
    }

    //Toggles the door open or closed and choosea direction that swings away from the player

    public void Interact(Transform interactor)   // <-- add this back
    {
        lastSideFront = IsInteractorInFront(interactor);

        StopAllCoroutines();
        if (isOpen)
            StartCoroutine(CloseDoor());
        else
            StartCoroutine(OpenDoor());
    }
    // Returns true if the interactor is on the door's forward side
    private bool IsInteractorInFront(Transform interactor)
    {
        Vector3 toInteractor = interactor.position - door.position;
        return Vector3.Dot(door.forward, toInteractor) > 0f;
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

        singleAudio.PlaySFX("close door");

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, closeAngle)) > 0.1f)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, closeAngle, Time.deltaTime * speed);
            door.localRotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
    }
}
