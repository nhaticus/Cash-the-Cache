// Video used:https://www.youtube.com/watch?v=3xSYkFdQiZ0&t=70s

// This script controls the speedometer of the car in Unity.
// It updates the needle position based on the car's speed and creates speed labels on the speedometer.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Speedometer : MonoBehaviour
{
    private const float MAX_SPEED_ANGLE = -90f;
    private const float ZERO_SPEED_ANGLE = 0;

    private Transform needTransform;
    [SerializeField] GameObject speedLabelTemplate;

    private float speedMax;
    private float currentSpeed;

    [SerializeField] private CarController carController;

    private void Awake()
    {
        needTransform = transform.Find("Needle");
        speedLabelTemplate.gameObject.SetActive(false);

        currentSpeed = 0f;
        speedMax = 100f;

        CreateSpeedLabels();
    }

    void Update()
    {
        currentSpeed = carController.GetCurrentSpeedKMH();
        if (currentSpeed > speedMax) 
            currentSpeed = speedMax;

        needTransform.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
    }

    private void CreateSpeedLabels() { 
        int labelAmount = 5;
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;
        for (int i = 0; i<= labelAmount; i++) {
            GameObject speedLabel = Instantiate(speedLabelTemplate, transform);
            float labelSpeedNormalized = (float)i / labelAmount;
            float speedLabelAngle = ZERO_SPEED_ANGLE + labelSpeedNormalized * totalAngleSize;
            speedLabel.transform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            Transform speedText = speedLabel.transform.Find("SpeedText");
            speedText.GetComponent<TMP_Text>().text = Mathf.RoundToInt((1f-labelSpeedNormalized) * speedMax).ToString();
            speedText.GetComponent<TMP_Text>().fontSize = 20;
            speedText.eulerAngles = Vector3.zero;
            speedLabel.SetActive(true);
        }
    }

    private float GetSpeedRotation() {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = currentSpeed / speedMax;

        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }

}
