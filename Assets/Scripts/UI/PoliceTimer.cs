using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    public float maxTime = 180f;
    public float minTime = 105f;
    public float timeDecrease = 25f;
    float defaultFontSize;
    float timeLeft;
    public bool timerOn = true;
    public TMP_Text Timer_display;

    // police stuff
    [SerializeField] GameObject police;
    [SerializeField] int numPoliceToSpawn = 1;
    [SerializeField] Transform[] spawnPos;

    [SerializeField] private SingleAudio policeAudio;

    private Vector3 originalPosition;
    private bool isTimerPaused = false; // Track if timer is paused

    private void Start()
    {
        GameManager.Instance.OnNPCLeaving += TickDownTimer;
        timeLeft = maxTime - (GameManager.Instance.numRuns * timeDecrease);
        if (timeLeft < minTime)
            timeLeft = minTime;
        originalPosition = Timer_display.rectTransform.localPosition; // Store original position
        defaultFontSize = Timer_display.fontSize;
    }

    void Update()
    {
        if (timerOn && !isTimerPaused)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
            }
            else
            {
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

        // Change font size, color, and add shaking effect when less than 1 minute remaining
        if (timeLeft < 60)
        {
            Timer_display.fontSize = defaultFontSize * 1.2f;
            Timer_display.color = Color.red;

            // Apply shaking effect
            float shakeAmount = 5 / (timeLeft / 30);
            if(shakeAmount > 9)
                shakeAmount = 9;
            Timer_display.rectTransform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
        }
        else
        {
            Timer_display.fontSize = defaultFontSize;
            Timer_display.color = Color.white;

            // Reset position when time is above 1 minute
            Timer_display.rectTransform.localPosition = originalPosition;
        }
    }

    void onTimerUp()
    {
        policeAudio.PlaySFX("police_radio");

        // Send in police at random spawn positions
        for (int i = 0; i < numPoliceToSpawn; i++)
        {
            Instantiate(police, spawnPos[Random.Range(0, spawnPos.Length)]);
        }

        // Lower next spawn time
        maxTime /= 1.75f;
        if (maxTime < 30)
            maxTime = 30;

        timeLeft = maxTime;
        timerOn = true;
    }

    void TickDownTimer()
    {
        int timeOff = 30;
        if (timeLeft - timeOff > timeOff)
        {
            timeLeft -= timeOff;
        }
        else if (timeLeft - timeOff <= timeOff && timeLeft >= timeOff)
        {
            timeLeft = timeOff;
        }
        StartCoroutine(TimerChangeEffect());
    }

    IEnumerator TimerChangeEffect()
    {
        float shakeAmount = 4;
        float shakeTime = 1f;
        while (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            Timer_display.rectTransform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return null;
        }
        Timer_display.rectTransform.localPosition = originalPosition;
    }

    // **Pause Timer when Player enters collider**
    public void PauseTimer()
    {
        isTimerPaused = true;
    }

    // **Resume Timer when Player exits collider**
    public void ResumeTimer()
    {
        isTimerPaused = false;
    }
}
