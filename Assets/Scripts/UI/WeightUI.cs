using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class WeightUI : MonoBehaviour
{
    [SerializeField] TMP_Text weightText, maxWeightText;
    [SerializeField] RectTransform fillBarTransform;
    PlayerInteract playerInteract;

    public void Initialize(GameObject _player)
    {
        playerInteract = _player.GetComponentInChildren<PlayerInteract>();
        playerInteract.ItemTaken.AddListener(UpdateWeightDisplay);
        weightText.text = PlayerManager.Instance.getWeight().ToString();
        maxWeightText.text = PlayerManager.Instance.getMaxWeight().ToString();
    }

    public void UpdateWeightDisplay()
    {
        weightText.text = PlayerManager.Instance.getWeight().ToString();

        float percentage = PlayerManager.Instance.getWeight() / (float) PlayerManager.Instance.getMaxWeight();
        Debug.Log("get weight: " + PlayerManager.Instance.getWeight());
        Debug.Log("MAX weight: " + PlayerManager.Instance.getMaxWeight());
        Debug.Log(percentage + "%");
        float newHeight = GetComponent<RectTransform>().sizeDelta.y * percentage;
        Debug.Log("newHeight: " + newHeight);
        fillBarTransform.sizeDelta = new Vector2(fillBarTransform.sizeDelta.x, newHeight);
    }
}
