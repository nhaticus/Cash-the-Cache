using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuOnEnable : MonoBehaviour
{
    [SerializeField] MainMenu mainMenu;
    [SerializeField] EventSystem eventSystem;

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(mainMenu.prevButton);
    }
}
