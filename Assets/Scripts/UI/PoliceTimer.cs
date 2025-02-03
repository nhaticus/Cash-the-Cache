using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    public float timeLeft = 300f;
    public bool timerOn = true;
    public TMP_Text Timer_display;

    // Update is called once per frame
    void Update()
    {
        if(timerOn){
            if(timeLeft > 0){
                timeLeft -= Time.deltaTime;
            } else{
                timeLeft = 0;
                timerOn = false;
            }
            updateTimerDisplay();
        }
    }

    void updateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        string timeText = minutes + ":" + seconds.ToString("00");

        Timer_display.text = minutes + ":" + seconds.ToString("00");
    }

    void onTimerUp(){
        // Sent in police
    }
}
