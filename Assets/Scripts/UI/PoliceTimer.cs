using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceTimer : MonoBehaviour
{
    public float timeLeft = 300f;
    public bool timerOn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        }
    }

    void OnGUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        string timeText = minutes + ":" + seconds.ToString("00");
        
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 200, 50), timeText, style);
    }

    void onTimerUp(){
        // Sent in police
    }
}
