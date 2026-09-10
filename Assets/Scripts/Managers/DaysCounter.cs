using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DaysCounter : MonoBehaviour
{
    [SerializeField] int daysLeft = 5, daysTotal = 45;
    [SerializeField] TMP_Text daysText;
    void Start()
    {
        if(daysLeft <= 0)
        {
            // end game
        }
    }

    private void Update()
    {
        daysText.text = daysLeft.ToString();
        SetRotation();
    }

    // slowly rotate counter when fewer days remaining
    void SetRotation()
    {
        float rotAmount = daysTotal / daysLeft;
        rotAmount = Mathf.Max(10, rotAmount);
        transform.localRotation = Quaternion.Euler(0, 0, rotAmount);
    }

}
