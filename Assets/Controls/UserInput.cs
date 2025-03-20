using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput Instance;
    PlayerInput playerInput;

    // controls to detect
    public Vector2 MoveInput { get; private set; }
    public bool Interact { get; private set; }
    public bool MenuExit { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        playerInput = GetComponent<PlayerInput>();
    }
}
