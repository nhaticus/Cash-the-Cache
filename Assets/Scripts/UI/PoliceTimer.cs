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

    private void Awake()
    {
        GameManager.Instance.OnNPCLeaving += TickDownTimer;
    }

    private Vector3 originalPosition;


    private void Start()
    {
        timeLeft = maxTime;
        originalPosition = Timer_display.rectTransform.localPosition; // Store original position
    }

    void Update()
    {
        if (timerOn)
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

        // Change font size, color, and add shaking effect when less than 2 minutes remaining
        if (timeLeft < 60)
        {
            Timer_display.fontSize = 70; // Increase font size
            Timer_display.color = Color.red; // Change text color to red

            // Apply shaking effect
            float shakeAmount = 5f; // Adjust for more or less shaking
            Timer_display.rectTransform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
        }
        else
        {
            Timer_display.fontSize = 46; // Default font size
            Timer_display.color = Color.white; // Default text color

            // Reset position when time is above 2 minutes
            Timer_display.rectTransform.localPosition = originalPosition;
        }
    }

    void onTimerUp()
    {
        // Send in police at random spawn positions
        for (int i = 0; i < numPoliceToSpawn; i++)
        {
            AudioManager.Instance.PlaySFX("police_radio");
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
    }
}
