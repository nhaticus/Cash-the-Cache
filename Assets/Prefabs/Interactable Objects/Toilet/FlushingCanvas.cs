//LockPickingCanvas handles the lockpicking minigame logic for the safe
// Player interacts with a set of pins that need to be pressed in a specific order to unlock the safe
// Difficulty determines the number of pins and the complexity of the lockpicking
// Dynamic pin creation and canvas creation
// Each pin is assigned a random order to be pressed
// Pins flash in the correct order to guide the player
// Player has a limited number of attempts to unlock the safe
// If the player fails and runs out of attempts then the safe is permantly locked and a message is shown
// If the player successfully unlocks the safe, they can loot the contents inside

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FlushingCanvas : MonoBehaviour
{
    [Header("Flushing Settings")]
    public int difficulty;
    [SerializeField] GameObject handlePrefab; // Prefab for the pin button
    [SerializeField] Slider powerSlider;
    [HideInInspector] public UnityEvent toiletOpened;
    [SerializeField] GameObject targetObject;
    [SerializeField] GameObject controlledObject;
    [SerializeField] float speed;
    [SerializeField] GameObject button;
    [SerializeField] float lowerBound;
    [SerializeField] float upperBound;
    private void Start()
    {
        powerSlider.value = 0;
    }

    private void Update()
    {
        ApplyForceToControl((-0.5f)*speed);
        if(powerSlider.value >= 1)
        {
            toiletOpened.Invoke();
            ExitToilet();
        }
        if ((controlledObject.GetComponent<RectTransform>().localPosition.x < lowerBound && button.GetComponent<FlushButton>().SENDIT == false) || (button.GetComponent<FlushButton>().SENDIT && controlledObject.GetComponent<RectTransform>().localPosition.x > upperBound))
        {
            controlledObject.GetComponent<Rigidbody>().drag = 9999999999999999999;
        }
        else
        {
            controlledObject.GetComponent<Rigidbody>().drag = 1;
        }
    }
    // Dynamically creates pins based on the difficulty level
    

    public void ExitButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        // Disable the canvas instead of destroying it
        gameObject.SetActive(false); // This will keep the state of the pins and combo intact
    }

    public void ExitToilet()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject); // Destroy the canvas when exiting
    }

    private void ApplyForceToControl(float force)
    {
        controlledObject.GetComponent<Rigidbody>().AddForce(new Vector3(force * Time.deltaTime, 0f, 0f), ForceMode.Force);
    }

    public void FlushButton()
    {
        controlledObject.GetComponent<Rigidbody>().AddForce(new Vector3(speed * Time.deltaTime, 0f, 0f), ForceMode.Force);
    }
}