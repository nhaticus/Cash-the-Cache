using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] RectTransform background;
    float backgroundHeight;

    [Header("Settings")]
    [SerializeField] float scrollSpeed = 110f;
    [SerializeField] float pauseTime = 0.8f;

    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        backgroundHeight = background.rect.height;
    }

    private void OnEnable()
    {
        StartCoroutine(BeginScroll());
    }

    public IEnumerator BeginScroll()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        yield return new WaitForSeconds(pauseTime);

        while (true)
        {
            rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

            // teleport back to bottom of credits background to loop again
            if (rectTransform.anchoredPosition.y >= rectTransform.rect.height)
            {
                yield return new WaitForSeconds(pauseTime);
                rectTransform.anchoredPosition = new Vector2(0, -backgroundHeight);
            }
            yield return null;
        }
    }
}
