using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectionBarController : MonoBehaviour
{
    [Header("UI")]
    public Image detectionBar;         // The Filled Image
    public Color flashColor = Color.yellow;

    [Header("Detection Values")]
    public float currentDetection = 0f;
    public float maxDetection = 2f;    // or however long it takes to fill


    [Header("NPC References")]
    // Manually assign these in the Inspector
    public List<NPCsBehavior> allNPCs = new List<NPCsBehavior>();

    private bool isFlashing = false;
    

    // Update is called once per frame
    void Update()
    {
        if (detectionBar == null) return;
        // Update the bar fill

        float highestRatio = 0f;
        foreach (NPCsBehavior npc in allNPCs)
        {
            if (npc == null) continue;
            // Each NPC might have a different sightTimer / sightCountdown
            float ratio = npc.GetDetectionRatio(); // e.g. sightTimer / sightCountdown
            if (ratio > highestRatio)
                highestRatio = ratio;
        }

        // 2) Update the bar fill
        detectionBar.fillAmount = highestRatio;
        // 3) If bar is full (>= 1), flash
        if (highestRatio >= 1f && !isFlashing)
        {
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

        // Reset color, fill amount, and detection
        detectionBar.color = originalColor;
        detectionBar.fillAmount = 0f;
        currentDetection = 0f;

        isFlashing = false;
    }

    public void SetDetectionValue(float value)
    {
        currentDetection = Mathf.Clamp(value, 0f, maxDetection);
    }

    public void IncreaseDetection(float amount)
    {
        currentDetection = Mathf.Clamp(currentDetection + amount, 0f, maxDetection);
    }

    public void ResetDetection()
    {
        currentDetection = 0f;
        detectionBar.fillAmount = 0f;
    }

}
