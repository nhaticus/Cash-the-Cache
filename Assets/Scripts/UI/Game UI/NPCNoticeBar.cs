using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class NPCNoticeBar : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the Slider component here")]
    public Slider noticeSlider;

    [Header("Flash Settings (when full)")]
    [Tooltip("Seconds between on/off flashes; set to 0 for no flash")]
    public float blinkInterval = 0f;

    [Header("Flash Colors")]
    [Tooltip("Color when the bar is 'on' (full)")]
    public Color flashOnColor = Color.yellow;
    [Tooltip("Color when the bar is 'off' (alternating)")]
    public Color flashOffColor = Color.magenta;

    // Internal state
    private NPCsBehavior npcBehavior;
    private Image fillImage;
    private Coroutine flashCoroutine = null;

    private void Awake()
    {
        // Find the NPCsBehavior on this NPC (somewhere in the parents)
        npcBehavior = GetComponentInParent<NPCsBehavior>();
        if (npcBehavior == null)
            Debug.LogError($"[NPCNoticeBar] No NPCsBehavior found in parents of '{name}'.");

        // Grab the Fill Image so we can change its color when flashing
        if (noticeSlider != null && noticeSlider.fillRect != null)
        {
            fillImage = noticeSlider.fillRect.GetComponent<Image>();
        }
        else
        {
            Debug.LogError($"[NPCNoticeBar] Please assign the Slider→Fill Rect in the Inspector on '{name}'.");
        }

        // Initialize slider range/value
        if (noticeSlider != null)
        {
            noticeSlider.minValue = 0f;
            noticeSlider.maxValue = 1f;
            noticeSlider.value = 0f;
        }
    }

    private void Update()
    {
        if (npcBehavior == null || noticeSlider == null)
            return;

        // 1) Get the raw detection ratio (0 → 1). In NPCsBehavior, sightTimer decays slowly,
        //    so rawRatio will also drop gradually rather than snapping to 0.
        float rawRatio = npcBehavior.GetDetectionRatio();

        // 2) Assign that raw ratio directly to the slider value.
        noticeSlider.value = rawRatio;

        // 3) Handle optional flashing when bar is full
        if (blinkInterval > 0f)
        {
            // Start flashing once it reaches 1.0
            if (rawRatio >= 1f && flashCoroutine == null)
            {
                flashCoroutine = StartCoroutine(FlashFill());
            }
            // Stop flashing as soon as it dips below 1.0
            else if (rawRatio < 1f && flashCoroutine != null)
            {
                StopFlashing();
            }
        }
    }

    /// <summary>
    /// Flashes the fill image back and forth between flashOnColor and flashOffColor.
    /// </summary>
    private System.Collections.IEnumerator FlashFill()
    {
        bool toggle = false;
        while (true)
        {
            if (fillImage != null)
                fillImage.color = toggle ? flashOnColor : flashOffColor;

            toggle = !toggle;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    /// <summary>
    /// Stops the flashing coroutine and restores the fill image to flashOnColor.
    /// </summary>
    private void StopFlashing()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            if (fillImage != null)
                fillImage.color = flashOnColor;
        }
    }
}
