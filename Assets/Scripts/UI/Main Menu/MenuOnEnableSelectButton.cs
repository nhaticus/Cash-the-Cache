using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuOnEnableSelectButton : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] EventSystem eventSystem;

    private void OnEnable()
    {
        eventSystem.SetSelectedGameObject(button);
    }
}
