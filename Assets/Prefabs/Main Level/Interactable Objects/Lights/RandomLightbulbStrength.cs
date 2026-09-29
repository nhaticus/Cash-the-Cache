using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class RandomLightbulbStrength : MonoBehaviour
{
    [Header("Strength")]
    [SerializeField] int minStrength;
    [SerializeField] int maxStrength;

    [Header("Alternators")]
    [SerializeField] int strengthVariation; // makes each child light a little different

    [Range(0.0f, 1.0f)]
    [SerializeField] float lightOffPercentage = 0.4f;

    void Start()
    {
        // get all child lights
        HDAdditionalLightData[] lights = GetComponentsInChildren<HDAdditionalLightData>();

        // choose if light is off
        bool lightsOff = Random.Range(0f, 1f) <= lightOffPercentage;

        // set a "light range"
        int lightStrength = Random.Range(minStrength, maxStrength);

        // set all lights to within that light range
        foreach (HDAdditionalLightData light in lights)
        {
            if (lightsOff)
                light.gameObject.SetActive(false);
            else
            {
                int strength = lightStrength + Random.Range(-strengthVariation, strengthVariation);
                strength = Mathf.Min(maxStrength, strength); // prevent strength from going past max strength
                light.SetIntensity(strength, LightUnit.Lumen);
            }
                

        }
    }
}
