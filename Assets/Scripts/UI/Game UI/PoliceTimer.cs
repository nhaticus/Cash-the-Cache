using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public TMP_Text policeMessageText;

    // police stuff
    [SerializeField] GameObject police;
    [SerializeField] int numPoliceToSpawn = 1;
    [SerializeField] Transform[] spawnPos;

    [SerializeField] SingleAudio policeAudio;

    [SerializeField] GameObject blueSquare, redSquare;

    private Vector3 originalPosition;
    private bool isTimerPaused = false; // Track if timer is paused

    private void Start()
    {
        GameManager.Instance.OnNPCLeaving += TickDownTimer;
        timeLeft = maxTime;
        if (timeLeft < minTime)
            timeLeft = minTime;
        originalPosition = Timer_display.rectTransform.localPosition; // Store original position
        defaultFontSize = Timer_display.fontSize;

        blueSquare.SetActive(false);
        redSquare.SetActive(false);
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
        Timer_display.rectTransform.localPosition = originalPosition;
        StartCoroutine(PoliceAlert());
        StartCoroutine(FlashWarningText());

        // Send in police at random spawn positions
        for (int i = 0; i < numPoliceToSpawn; i++)
        {
            Instantiate(police, spawnPos[Random.Range(0, spawnPos.Length)]);
        }
    }

    IEnumerator PoliceAlert()
    {
        policeAudio.PlaySFX("police_radio");

        Color blueOn = blueSquare.GetComponent<Image>().color;
        blueOn.a = 0.9f;
        Color blueOff = blueSquare.GetComponent<Image>().color;
        blueOff.a = 0.4f;

        Color redOn = redSquare.GetComponent<Image>().color;
        redOn.a = 0.9f;
        Color redOff = redSquare.GetComponent<Image>().color;
        redOff.a = 0.4f;

        Image blueImage = blueSquare.GetComponent<Image>();
        Image redImage = redSquare.GetComponent<Image>();
        blueSquare.SetActive(true);
        redSquare.SetActive(true);

        timerOn = false;
        for(int i = 0; i < 5; i++)
        {
            blueImage.color = blueOn;
            redImage.color = redOff;
            yield return new WaitForSeconds(0.3f);
            blueImage.color = blueOff;
            redImage.color = redOn;
            yield return new WaitForSeconds(0.3f);
        }

        blueSquare.SetActive(false);
        redSquare.SetActive(false);

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
        StartCoroutine(TimerDecreaseEffect());
    }

    IEnumerator TimerDecreaseEffect()
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
    IEnumerator FlashWarningText()
    {
        float flashDuration = 3f;    // Total duration for flashing
        float flashInterval = 0.3f;  // Time interval between color changes
        float elapsed = 0f;

        // Set initial text message
        policeMessageText.text = "Police are coming!";

        while (elapsed < flashDuration)
        {
            // Alternate between blue and red
            if (Mathf.FloorToInt(elapsed / flashInterval) % 2 == 0)
            {
                policeMessageText.color = Color.blue;
            }
            else
            {
                policeMessageText.color = Color.red;
            }
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // Clear message and reset color after flashing
        policeMessageText.text = "";
        policeMessageText.color = Color.white;
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
