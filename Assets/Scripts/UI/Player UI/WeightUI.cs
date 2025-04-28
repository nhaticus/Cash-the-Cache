using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeightUI : MonoBehaviour
{
    [SerializeField] TMP_Text weightText, maxWeightText;
    [SerializeField] RectTransform fillBarTransform;
    PlayerInteract playerInteract;
    bool uiChanging = false;

    public void Initialize(GameObject _player)
    {
        playerInteract = _player.GetComponentInChildren<PlayerInteract>();
        playerInteract.ItemTaken.AddListener(ItemTaken);
        weightText.text = PlayerManager.Instance.getWeight().ToString();
        maxWeightText.text = PlayerManager.Instance.getMaxWeight().ToString();
    }

    public void ItemTaken(bool taken)
    {
        if (taken)
        {
            UpdateWeightDisplay();
        }
        else if (!uiChanging)
        {
            StartCoroutine(WeightJiggle());
        }
    }

    public void UpdateWeightDisplay()
    {
        weightText.text = PlayerManager.Instance.getWeight().ToString();

        float percentage = PlayerManager.Instance.getWeight() / (float) PlayerManager.Instance.getMaxWeight();
        float newHeight = GetComponent<RectTransform>().sizeDelta.y * percentage;
        fillBarTransform.sizeDelta = new Vector2(fillBarTransform.sizeDelta.x, newHeight);
    }

    // to show you cannot pick up item
    public IEnumerator WeightJiggle()
    {
        uiChanging = true;
        float prevSize = weightText.fontSize;
        weightText.color = UnityEngine.Color.red;
        weightText.fontSize = weightText.fontSize * 1.25f;
        yield return new WaitForSeconds(0.35f);
        weightText.color = UnityEngine.Color.black;
        weightText.fontSize = prevSize;
        uiChanging = false;
    }
}
