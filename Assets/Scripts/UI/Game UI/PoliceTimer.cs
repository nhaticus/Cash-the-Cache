using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    [Header("Timer")]
    public float maxTime = 20f;
    float timeLeft;
    bool timerOn = false;
    public TMP_Text Timer_display;
    [SerializeField] Color timerDefaultColor, timerAlertColor; // remove later

    [Header("Police")]
    [SerializeField] SingleAudio policeAudio;

    [Header("Sirens")]
    [SerializeField] Image blueSquare, redSquare;

    private Vector3 originalPosition;

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;

        timeLeft = maxTime;
        originalPosition = Timer_display.rectTransform.localPosition; // Store original position
        updateTimerDisplay();

        /*  When NPC leaves start timer  */
        GameManager.Instance.OnNPCLeaving += StartTimer;
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
                // stop decreasing timer and do TimerUp event
                timeLeft = 0;
                timerOn = false;
                TimerFinished();
            }

            updateTimerDisplay();
        }
    }

    void updateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        Timer_display.text = minutes + ":" + seconds.ToString("00");

        // Change color and add shaking effect
        Timer_display.color = timerAlertColor;

        // Apply shaking effect
        float shakeAmount = 5;
        Timer_display.rectTransform.localPosition = originalPosition + (Vector3) Random.insideUnitCircle * shakeAmount;
    }

    void StartTimer()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        timerOn = true;
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

    /// <summary>
    /// Effect that turns on and off blue and red squares
    /// </summary>
    IEnumerator PoliceAlert()
    {
        policeAudio.PlaySFX("police_radio");

        Color blueOn = blueSquare.color;
        blueOn.a = 0.9f;
        Color blueOff = blueSquare.color;
        blueOff.a = 0.3f;
        
        Color redOn = redSquare.color;
        redOn.a = 0.9f;
        Color redOff = redSquare.color;
        redOff.a = 0.3f;

        blueSquare.gameObject.SetActive(true);
        redSquare.gameObject.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            blueSquare.color = blueOn;
            redSquare.color = redOff;
            yield return new WaitForSeconds(0.3f);
            blueSquare.color = blueOff;
            redSquare.color = redOn;
            yield return new WaitForSeconds(0.3f);
        }

        // restart timer to call more police
        timeLeft = maxTime;
        timerOn = true;
        
    }

    void TimerFinished()
    {
        // check if game over to not keep spawning police
        if(GameManager.Instance.CurrentState != GameManager.GameState.Over)
        {
            Timer_display.rectTransform.localPosition = originalPosition;
            StartCoroutine(PoliceAlert());
            GameManager.Instance.CallSpawnPolice();
        }
        
    }

}
