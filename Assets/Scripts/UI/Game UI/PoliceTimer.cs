using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoliceTimer : MonoBehaviour
{
    [Header("Initial Countdown (seconds)")]
    [Tooltip("Default time before police arrive if no notice bar trigger occurs.")]
    public float initialTime = 180f;  // 3 minutes

    [Header("Override Countdown (seconds)")]
    [Tooltip("Time from the moment the notice bar hits full until police arrive.")]
    public float overrideTime = 10f;  // 10 seconds once bar is full

    float defaultFontSize;
    float timeLeft;
    public bool timerOn = true;       // Start counting down from initialTime immediately

    [Header("UI References")]
    public TMP_Text Timer_display;
    public TMP_Text policeMessageText;

    [Header("Police Spawn Settings")]
    [SerializeField] GameObject police;
    [SerializeField] int numPoliceToSpawn = 1;
    [SerializeField] Transform[] spawnPos;

    [SerializeField] SingleAudio policeAudio;
    [SerializeField] GameObject blueSquare, redSquare;

    private Vector3 originalPosition;
    private bool isTimerPaused = false;

    private void Start()
    {
        // Subscribe so that NPCLeaving can shave off time if needed:
        GameManager.Instance.OnNPCLeaving += TickDownTimer;

        // Begin with the 3‐minute countdown
        timeLeft = initialTime;

        originalPosition = Timer_display.rectTransform.localPosition;
        defaultFontSize = Timer_display.fontSize;

        blueSquare.SetActive(false);
        redSquare.SetActive(false);
    }

    private void Update()
    {
        if (timerOn && !isTimerPaused)
        {
            if (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
            }
            else
            {
                timeLeft = 0f;
                timerOn = false;
                SpawnPoliceNow();
            }
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        Timer_display.text = minutes + ":" + seconds.ToString("00");

        // When under 1 minute, enlarge/shake
        if (timeLeft < 60f)
        {
            Timer_display.fontSize = defaultFontSize * 1.2f;
            Timer_display.color = Color.red;

            float shakeAmount = 5f / (timeLeft / 30f);
            if (shakeAmount > 9f) shakeAmount = 9f;
            Timer_display.rectTransform.localPosition =
                originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
        }
        else
        {
            Timer_display.fontSize = defaultFontSize;
            Timer_display.color = Color.white;
            Timer_display.rectTransform.localPosition = originalPosition;
        }
    }

    private void SpawnPoliceNow()
    {
        // Reset UI position
        Timer_display.rectTransform.localPosition = originalPosition;

        // Start alert & flash coroutines
        StartCoroutine(PoliceAlert());
        StartCoroutine(FlashWarningText());

        // Actually instantiate police
        for (int i = 0; i < numPoliceToSpawn; i++)
        {
            Instantiate(police, spawnPos[Random.Range(0, spawnPos.Length)]);
        }
    }

    private IEnumerator PoliceAlert()
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

        // Flash them 5 times before ending
        for (int i = 0; i < 5; i++)
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
    }

    private IEnumerator FlashWarningText()
    {
        float flashDuration = 3f;
        float flashInterval = 0.3f;
        float elapsed = 0f;

        policeMessageText.text = "Police are coming!";

        while (elapsed < flashDuration)
        {
            if (Mathf.FloorToInt(elapsed / flashInterval) % 2 == 0)
                policeMessageText.color = Color.blue;
            else
                policeMessageText.color = Color.red;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        policeMessageText.text = "";
        policeMessageText.color = Color.white;
    }

    private void TickDownTimer()
    {
        // If an NPC leaves early, shave off 30 seconds—just as before
        int timeOff = 30;
        if (timeLeft - timeOff > timeOff)
            timeLeft -= timeOff;
        else if (timeLeft - timeOff <= timeOff && timeLeft >= timeOff)
            timeLeft = timeOff;

        StartCoroutine(TimerDecreaseEffect());
    }

    private IEnumerator TimerDecreaseEffect()
    {
        float shakeAmount = 4f;
        float shakeTime = 1f;
        while (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            Timer_display.rectTransform.localPosition =
                originalPosition + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return null;
        }
        Timer_display.rectTransform.localPosition = originalPosition;
    }

    /// <summary>
    /// Call this once the notice bar hits full. It overrides the current countdown
    /// and forces the police to spawn in exactly 10 seconds.
    /// </summary>
    public void TriggerPoliceTimer()
    {
        timerOn = true;
        timeLeft = overrideTime; // jump straight to 10 seconds
    }

    public void PauseTimer()
    {
        isTimerPaused = true;
    }

    public void ResumeTimer()
    {
        isTimerPaused = false;
    }
}
