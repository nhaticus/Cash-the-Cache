using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeightUI : MonoBehaviour
{
    [SerializeField] TMP_Text weightText, maxWeightText;
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
    }
}
