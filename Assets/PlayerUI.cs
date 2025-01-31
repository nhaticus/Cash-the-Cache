using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 * UI that follows the first person player view
 * 
 * keeps searching for player and once found gives following items a reference to player:
 * items:
 * weight
 */

public class PlayerUI : MonoBehaviour
{
    public GameObject player;
    WeightUI weightUI;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        while(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }
        weightUI = GetComponentInChildren<WeightUI>();
        weightUI.Initialize(player);
    }

}
