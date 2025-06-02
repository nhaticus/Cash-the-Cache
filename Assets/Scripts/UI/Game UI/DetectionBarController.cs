using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Detection Bar attached to NPCs
 * Just shows how much detection and has a flashing effect
 */

public class DetectionBarController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image detectionFill;         // The Filled Image
    [SerializeField] Color flashColor = Color.yellow;

    public void SetValue(float value)
    {
        detectionFill.fillAmount = value;
    }

    public IEnumerator FlashingEffect()
    {
        // Store the original color
        Color originalColor = detectionFill.color;

        float totalFlashTime = 3f;   // total time to flash
        float interval = 0.2f;       // how long each color stays active before switching
        float elapsedTime = 0f;     // total time passed
        bool toggleColor = false;

        // Keep toggling between Yellow and Red for the specified time
        while (elapsedTime < totalFlashTime)
        {
            detectionFill.color = toggleColor ? flashColor : originalColor;
            toggleColor = !toggleColor;

            yield return new WaitForSeconds(interval);
            elapsedTime += interval;
        }
    }
}
