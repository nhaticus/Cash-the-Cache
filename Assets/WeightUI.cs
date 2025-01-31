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
        playerInteract = _player.GetComponent<PlayerInteract>();
        playerInteract.ItemTaken.AddListener(PlayerTake);
        PlayerTake(); // change weight
    }

    public void PlayerTake()
    {
        weightText.text = playerInteract.weight.ToString();
        maxWeightText.text = playerInteract.maxWeight.ToString();
    }
}
