using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * https://www.youtube.com/watch?v=qXbjyzBlduY&ab_channel=SasquatchBStudios
 */

public class UserInput : MonoBehaviour
{
    public static UserInput Instance;
    
    // controls to detect
    public Vector2 Move { get; private set;  }
    public Vector2 Camera { get; private set; }
    public bool Interact { get; private set; }
    public bool Punch { get; private set; }
    public bool Inventory { get; private set; }
    public bool Cancel { get; private set; }
    public bool Pause { get; private set; }

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction cameraAction;
    InputAction interactAction;
    InputAction punchAction;
    InputAction inventoryAction;
    InputAction cancelAction;
    InputAction pauseAction;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerInput = GetComponent<PlayerInput>();
        SetupInputActions();
    }
    private void Update()
    {
        UpdateInput();
    }

    void SetupInputActions()
    {
        moveAction = playerInput.actions["Move"];
        cameraAction = playerInput.actions["Camera"];
        interactAction = playerInput.actions["Interact"];
        punchAction = playerInput.actions["Punch"];
        inventoryAction = playerInput.actions["Inventory"];
        cancelAction = playerInput.actions["Cancel"];
        pauseAction = playerInput.actions["Pause"];
    }

    void UpdateInput()
    {
        Move = moveAction.ReadValue<Vector2>();
        Camera = cameraAction.ReadValue<Vector2>();
        Interact = interactAction.WasPressedThisFrame();
        Punch = punchAction.WasPressedThisFrame();
        Inventory = inventoryAction.WasPressedThisFrame();
        Cancel = cancelAction.WasPressedThisFrame();
        Pause = pauseAction.WasPressedThisFrame();
    }
}
