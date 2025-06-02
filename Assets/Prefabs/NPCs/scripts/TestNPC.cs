using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * A basic cube to test NPC Detection
 */

public class TestNPC : MonoBehaviour
{
    [SerializeField] TMP_Text report;

    void Start()
    {
        report.text = "normal";
    }

    public void PlayerNoticed(Vector3 player)
    {
        report.text = "notice: " + player;
    }

    public void PlayerRecognized()
    {
        report.text = "COMPLETE DETECTED";
    }

    // if the player moves away from sight
    public void PlayerLost()
    {
        report.text = "normal";
    }
}
