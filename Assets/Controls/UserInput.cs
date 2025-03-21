using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * https://www.youtube.com/watch?v=qXbjyzBlduY&ab_channel=SasquatchBStudios
 * 
 */

public class UserInput : MonoBehaviour
{
    public static UserInput Instance;
    
    // controls to detect
    public Vector2 MoveInput { get; private set; }
    public Vector2 CameraInput { get; private set; }
    public bool Interact { get; private set; }
    public bool Inventory { get; private set; }
    public bool MenuExit { get; private set; }
    public bool Pause { get; private set; }

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction cameraAction;
    InputAction interactAction;
    InputAction inventoryAction;
    InputAction exitAction;
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
        inventoryAction = playerInput.actions["Inventory"];
        exitAction = playerInput.actions["Menu Exit"];
        pauseAction = playerInput.actions["Pause"];
    }

    void UpdateInput()
    {
        MoveInput = moveAction.ReadValue<Vector2>();
        CameraInput = cameraAction.ReadValue<Vector2>();
        Interact = interactAction.WasPressedThisFrame();
        Inventory = inventoryAction.WasPressedThisFrame();
        MenuExit = exitAction.WasPressedThisFrame();
        Pause = pauseAction.WasPressedThisFrame();
    }
}
