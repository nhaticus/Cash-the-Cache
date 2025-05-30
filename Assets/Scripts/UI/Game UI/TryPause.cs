using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryPause : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    void Update()
    {
        if ((UserInput.Instance && UserInput.Instance.Pause) || (UserInput.Instance == null && Input.GetKeyDown(KeyCode.Escape)))
        {
            UIManager.Instance.TrySwapMenu(pauseMenu);
        }
    }
}
