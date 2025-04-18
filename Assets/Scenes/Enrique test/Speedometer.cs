using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private const float MAX_SPEED_ANGLE = -90f;
    private const float ZERO_SPEED_ANGLE = 0;

    private Transform needTransform;
    private Transform speedLabelTemplateTransform;



    private float speedMax;
    private float currentSpeed;

    [SerializeField] private CarController carController;

    // Start is called before the first frame update
    private void Awake()
    {
        needTransform = transform.Find("Needle");
        speedLabelTemplateTransform = transform.Find("SpeedLabelTemplate");
        speedLabelTemplateTransform.gameObject.SetActive(false);


        currentSpeed = 0f;
        speedMax = 100f;

        CreateSpeedLabels();
    }

    // Update is called once per frame
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
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalized = (float)i / labelAmount;
            float speedLabelAngle = ZERO_SPEED_ANGLE + labelSpeedNormalized * totalAngleSize;
            speedLabelTransform.eulerAngles = new Vector3(0, 0, speedLabelAngle);
            speedLabelTransform.Find("SpeedText").GetComponent<TMP_Text>().text = Mathf.RoundToInt((1f-labelSpeedNormalized) * speedMax).ToString();
            speedLabelTransform.Find("SpeedText").eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);
        }
    }

    private float GetSpeedRotation() {
        float totalAngleSize = ZERO_SPEED_ANGLE - MAX_SPEED_ANGLE;

        float speedNormalized = currentSpeed / speedMax;


        return ZERO_SPEED_ANGLE - speedNormalized * totalAngleSize;
    }


}
