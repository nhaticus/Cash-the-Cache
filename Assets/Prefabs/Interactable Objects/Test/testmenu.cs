using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testmenu : MonoBehaviour
{
    public void GoScene()
    {
        SceneManager.LoadScene("Test");
    }
}
