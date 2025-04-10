using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUI : MonoBehaviour
{
    public static GameInfoUI Instance;

    [SerializeField] Transform textSpawnArea;
    [SerializeField] GameObject textPrefab;
    [SerializeField] Scrollbar scrollbar;

    Queue<GameObject> textToRemove = new Queue<GameObject>();
    int maxHold = 7;

    private void Start()
    {
        Instance = this;
    }

    bool canRemove = false;
    float waitTime = 1.2f;
    private void Update()
    {
        if (canRemove)
        {
            if (textToRemove.Count > 0)
            {
                waitTime -= Time.deltaTime;
                if(waitTime <= 0)
                {
                    Destroy(textToRemove.Dequeue());
                    waitTime = 1.2f;
                }
            }
            else
            {
                canRemove = false;
            }
        }
    }

    public void AddMessage(string message)
    {
        GameObject txt = Instantiate(textPrefab, textSpawnArea);
        txt.GetComponent<TMP_Text>().text = message;
        textToRemove.Enqueue(txt);
        scrollbar.value = 0;
        canRemove = true;
        if(textToRemove.Count > maxHold)
        {
            Destroy(textToRemove.Dequeue());
        }
    }
}
