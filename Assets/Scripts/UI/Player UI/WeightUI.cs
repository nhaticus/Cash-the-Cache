using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeightUI : MonoBehaviour
{
    [SerializeField] TMP_Text currWeightText, maxWeightText;
    [SerializeField] Image fillBar;

    PlayerInteract playerInteract;
    bool uiChanging = false;

    public void Initialize(GameObject _player)
    {
        // event listeners
        playerInteract = _player.GetComponentInChildren<PlayerInteract>();
        playerInteract.ItemTaken.AddListener(ItemTaken);

        // set text
        currWeightText.text = PlayerManager.Instance.getWeight().ToString();
        maxWeightText.text = PlayerManager.Instance.getMaxWeight().ToString();

        // set fill
        float percentage = PlayerManager.Instance.getWeight() / (float)PlayerManager.Instance.getMaxWeight();
        fillBar.fillAmount = percentage;
    }

    public void ItemTaken(bool taken)
    {
        if (taken)
            UpdateWeightDisplay();
        else if (!uiChanging)
            StartCoroutine(WeightJiggle());
    }

    public void UpdateWeightDisplay()
    {
        if (PlayerManager.Instance)
        {
            currWeightText.text = PlayerManager.Instance.getWeight().ToString();

            float percentage = PlayerManager.Instance.getWeight() / (float)PlayerManager.Instance.getMaxWeight();
            fillBar.fillAmount = percentage;
        }
    }

    // show you cannot pick up item
    public IEnumerator WeightJiggle()
    {
        uiChanging = true;
        // save previous color and size
        Color prevColor = currWeightText.color;
        float prevSize = currWeightText.fontSize;

        // change size for a little
        currWeightText.color = Color.red;
        currWeightText.fontSize = currWeightText.fontSize * 1.25f;
        yield return new WaitForSeconds(0.35f);

        currWeightText.color = prevColor;
        currWeightText.fontSize = prevSize;
        uiChanging = false;
    }
}
