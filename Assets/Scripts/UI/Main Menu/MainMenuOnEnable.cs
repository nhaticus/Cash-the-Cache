using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuOnEnable : MonoBehaviour
{
    [SerializeField] MainMenu mainMenu;
    [SerializeField] EventSystem eventSystem;

    private void Start()
    {
        // enable cursor
        // dumb fix for map -> cutscene -> main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(mainMenu.prevButton);
    }
}
