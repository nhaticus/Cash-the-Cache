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
        npcDetection.PlayerNoticed += PlayerNoticed;
        npcDetection.PlayerRecognized += PlayerRecognized;
        npcDetection.PlayerStartLost += PlayerLost;
        npcDetection.PlayerCompleteLost += PlayerCompleteLost;
        report.text = "normal";
    }

    void PlayerNoticed(Vector3 player)
    {
        report.text = "notice player";
    }

    void PlayerRecognized()
    {
        report.text = "COMPLETE DETECTED";
    }

    // if the player moves away from sight
    void PlayerLost()
    {
        report.text = "player lost";
    }

    void PlayerCompleteLost()
    {
        report.text = "normal";
    }
}
