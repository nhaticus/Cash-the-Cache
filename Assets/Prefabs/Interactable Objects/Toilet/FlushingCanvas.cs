// Flushing canvas handles the flushing minigame, as well as the creation of the canvas
// Player has to sync up the target and the controlled box in order to raise the meter, and doing it enough flushes the toilet

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
        if ((controlledObject.GetComponent<RectTransform>().localPosition.x < lowerBound && button.GetComponent<FlushButton>().SENDIT == false) || 
            (button.GetComponent<FlushButton>().SENDIT && controlledObject.GetComponent<RectTransform>().localPosition.x > upperBound))
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

        Destroy(gameObject); // Destroy the canvas
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