using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public TaskListScript taskList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void task1Complete()
    {
        taskList.triggerTask1();
    }

    public void task2Complete()
    {
        taskList.triggerTask2();
    }

    public void task3Complete()
    {
        taskList.triggerTask3();
    }

}
