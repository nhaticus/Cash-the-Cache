using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    public float maxTime = 300f;
    public float timeLeft;
    public bool timerOn = true;
    public TMP_Text Timer_display;

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
        // Send in police
        timerOn = true;
        timeLeft = maxTime / 1.5f;

    }
}
