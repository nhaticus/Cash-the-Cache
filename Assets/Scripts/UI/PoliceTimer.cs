using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    public float maxTime = 180f;
    public float minTime = 60f;
    public float timeDecrease = 30f;
    float timeLeft;
    public bool timerOn = true;
    public TMP_Text Timer_display;
    float defaultTextSize;

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
        timeLeft = maxTime - (GameManager.Instance.numRuns * timeDecrease);
        if (timeLeft < minTime)
            timeLeft = minTime;
        originalPosition = Timer_display.rectTransform.localPosition; // Store original position
        defaultTextSize = Timer_display.fontSize;
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

        // Change font size, color, and add shaking effect when less than 1 minute remaining
        if (timeLeft < 60)
        {
            Timer_display.fontSize = defaultTextSize * 1.2f; // Increase font size
            Timer_display.color = Color.red; // Change text color to red

            // Apply shaking effect
            float shakeAmount = 6 / (timeLeft / 10); // Adjust for more or less shaking
            if (shakeAmount > 8)
                shakeAmount = 8;
            Timer_display.rectTransform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
        }
        else
        {
            Timer_display.fontSize = defaultTextSize; // Default font size
            Timer_display.color = Color.white; // Default text color

            // Reset position
            Timer_display.rectTransform.localPosition = originalPosition;
        }
    }

    void onTimerUp()
    {
        AudioManager.Instance.PlaySFX("police_radio");
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
    }
}
