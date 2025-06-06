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

    // steal item
    public void triggerTask1()
    {
        task1BG.color = new Color(255, 255, 255, 255);
        task1Text.color = new Color32(0, 198, 0, 255);
        task1Text.fontStyle = FontStyles.Strikethrough;
    }

    // deposit to van
    public void triggerTask2()
    {
        task2BG.color = new Color32(255, 255, 255, 255);
        task2Text.color = new Color32(0, 198, 0, 255);
        task2Text.fontStyle = FontStyles.Strikethrough;
    }

    // drive away
    public void triggerTask3()
    {
        task3BG.color = new Color32(255, 255, 255, 255);
        task3Text.color = new Color32(0, 198, 0, 255);
        task3Text.fontStyle = FontStyles.Strikethrough;
    }
}
