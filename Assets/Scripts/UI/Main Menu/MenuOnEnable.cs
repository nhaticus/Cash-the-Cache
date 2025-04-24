using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuOnEnable : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] EventSystem eventSystem;

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(button);
    }
}
