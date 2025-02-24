using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskListScript : MonoBehaviour
{

    [SerializeField]
    private Image task1BG;
    [SerializeField]
    private TextMeshProUGUI task1Text;

    [SerializeField]
    private Image task2BG;
    [SerializeField]
    private TextMeshProUGUI task2Text;

    [SerializeField]
    private Image task3BG;
    [SerializeField]       
    private TextMeshProUGUI task3Text;

    [SerializeField]
    private Image task4BG;
    [SerializeField]
    private TextMeshProUGUI task4Text;

    private void triggerTask1()
    {
        task1BG.color = new Color32(0, 198, 0, 255);
        task1Text.color = new Color32(0, 198, 0, 255);
        task1Text.fontStyle = FontStyles.Strikethrough;
    }

    private void triggerTask2()
    {

    }

    private void triggerTask3()
    {

    }

    private void triggerTask4()
    {

    }
}
