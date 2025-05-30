using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Manager to determine which UI to show
 * Mostly to help with preventing pausing when playing minigame
 */

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] GameObject currentMenu;
    bool canSwap = true;

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
    }

    public void TrySwapMenu(GameObject newMenu)
    {
        if (canSwap)
        {
            Destroy(currentMenu);
            currentMenu = Instantiate(newMenu);
            currentMenu.SetActive(true);
        }
    }

    public void SetSwappable(bool swap)
    {
        canSwap = swap;
    }

}
