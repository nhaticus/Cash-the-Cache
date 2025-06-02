using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestNPC : MonoBehaviour
{
    NPCDetection npcDetection;
    bool playerLost = false;
    float maxTime = 1f;
    [SerializeField] TMP_Text report;

    void Start()
    {
        npcDetection = GetComponent<NPCDetection>();
        npcDetection.PlayerRecognized += PlayerDetected;
        npcDetection.PlayerStartLost += PlayerLost;
        report.text = "normal path";
    }


    void Update()
    {
        if (playerLost)
        {
            maxTime -= Time.deltaTime;
            report.text = "stand still";
            if (maxTime <= 0)
            {
                report.text = "normal path";
                maxTime = 1;
                playerLost = false;
            }
            
        }
    }

    // when the slider value is full
    void PlayerDetected()
    {
        report.text = "running away and turn timer on";
    }

    // if the player moves away from sight
    void PlayerLost()
    {
        playerLost = true;
    }
}
