using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    public float maxTime = 300f;
    float timeLeft;
    public bool timerOn = true;
    public TMP_Text Timer_display;

    // police stuff
    [SerializeField] GameObject police;
    [SerializeField] int numPoliceToSpawn = 1;
    [SerializeField] Transform[] spawnPos;

    private void Start()
    {
        timeLeft = maxTime;
    }

    void Update()
    {
        if(timerOn){
            if(timeLeft > 0){
                timeLeft -= Time.deltaTime;
            } else{
                timeLeft = 0;
                timerOn = false;
                onTimerUp();
            }
            updateTimerDisplay();
        }
    }

    void updateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        Timer_display.text = minutes + ":" + seconds.ToString("00");
    }

    void onTimerUp(){
        // Send in police at random spawn positions
        for(int i = 0; i < numPoliceToSpawn; i++)
        {
            Instantiate(police, spawnPos[Random.Range(0, spawnPos.Length)]);
        }

        // lower next spawn time
        maxTime /= 1.75f;
        if(maxTime < 30)
            maxTime = 30;
        timeLeft = maxTime;
        timerOn = true;
    }
}
