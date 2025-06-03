using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectionBarController : MonoBehaviour
{
    [Header("UI")]
    public Image detectionBar;         // The Filled Image component that shows the notice bar
    public Color flashColor = Color.yellow;

    [Header("NPC References")]
    // Manually assign these in the Inspector (all NPCs whose detection you want to pool)
    public List<NPCsBehavior> allNPCs = new List<NPCsBehavior>();

    [Header("Police Timer Reference")]
    [Tooltip("Drag the PoliceTimer component (or its GameObject) here, so we can call TriggerPoliceTimer().")]
    public PoliceTimer policeTimer;

    private bool isFlashing = false;
    private bool hasTriggeredPolice = false;

    void Update()
    {
        if (detectionBar == null) return;

        // 1) Find the highest detection ratio among all NPCs
        float highestRatio = 0f;
        foreach (NPCsBehavior npc in allNPCs)
        {
            if (npc == null) continue;
            float ratio = npc.GetDetectionRatio();
            if (ratio > highestRatio)
                highestRatio = ratio;
        }

        // 2) Update the UI bar fill
        detectionBar.fillAmount = highestRatio;

        // 3) As soon as that pooled ratio hits 1.0, trigger the police timer once
        if (highestRatio >= 1f && !hasTriggeredPolice)
        {
            hasTriggeredPolice = true;

            // Start the global police countdown
            if (policeTimer != null)
                policeTimer.TriggerPoliceTimer();
            else
                Debug.LogWarning("No PoliceTimer assigned on DetectionBarController.");

            // Begin flashing the bar (optional)
            if (!isFlashing)
                StartCoroutine(FlashAndReset());
        }
    }

    private IEnumerator FlashAndReset()
    {
        isFlashing = true;

        // Store the original color
        Color originalColor = detectionBar.color;

        float totalFlashTime = 3f;   // total time to flash
        float interval = 0.2f;       // how long each color stays before toggling
        float elapsedTime = 0f;
        bool toggleColor = false;

        // Keep toggling between Yellow and Red for the specified time
        while (elapsedTime < totalFlashTime)
        {
            detectionBar.color = toggleColor ? Color.yellow : Color.red;
            toggleColor = !toggleColor;

            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }

        // Reset color and fill amount
        detectionBar.color = originalColor;
        detectionBar.fillAmount = 0f;

        isFlashing = false;
        hasTriggeredPolice = false;
    }
}
