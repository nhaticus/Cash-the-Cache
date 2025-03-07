using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    public float maxTime = 180f;
    public float minTime = 80f;
    public float timeDecrease = 25f;
    float timeLeft;
    public bool timerOn = true;
    public TMP_Text Timer_display;

    // police stuff
    [SerializeField] GameObject police;
    [SerializeField] int numPoliceToSpawn = 1;
    [SerializeField] Transform[] spawnPos;

    private Vector3 originalPosition;
    private bool isTimerPaused = false; // Track timer pause state
    private Coroutine pauseCoroutine; // Store coroutine reference

    private void Awake()
    {
        GameManager.Instance.OnNPCLeaving += TickDownTimer;
    }

    private void Start()
    {
        timeLeft = maxTime - (GameManager.Instance.numRuns * timeDecrease);
        if (timeLeft < minTime)
            timeLeft = minTime;
        originalPosition = Timer_display.rectTransform.localPosition; // Store original position
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
            Timer_display.fontSize = 70;
            Timer_display.color = Color.red;

            // Apply shaking effect
            float shakeAmount = 5f;
            Timer_display.rectTransform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
        }
        else
        {
            Timer_display.fontSize = 46;
            Timer_display.color = Color.white;

            // Reset position when time is above 1 minute
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

    // **NEW: Stop Timer for 3 Minutes when Player enters a collider**
    public void StopTimerFor3Minutes()
    {
        if (!isTimerPaused)
        {
            if (pauseCoroutine != null)
                StopCoroutine(pauseCoroutine);

            pauseCoroutine = StartCoroutine(PauseTimerForDuration(180f)); 
        }
    }

    public void ResumeTimer()
    {
        if (pauseCoroutine != null)
            StopCoroutine(pauseCoroutine);

        isTimerPaused = false;
    }

    private IEnumerator PauseTimerForDuration(float duration)
    {
        isTimerPaused = true;
        yield return new WaitForSeconds(duration);
        isTimerPaused = false;
    }
}
