using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class AnchoredRotation : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 3;

    [Header("Dependencies")]
    [SerializeField] Transform AnchorPoint;
    [SerializeField] float RotationRadius;

    float rotAmount = 0;
    float angle = 0;

    bool canRotate = true;

    private void Start() {
        rotAmount = 2 * Mathf.PI * (speed / 3) * Time.deltaTime;
    }

    public void SetRotationAmount(float speed) {
        this.speed = speed;
        rotAmount = 2 * Mathf.PI * (speed / 3) * Time.deltaTime;
    }
    
    void Update()
    {
        if (canRotate)
        {
            angle = (angle + rotAmount) % (2 * Mathf.PI);

            Vector2 newPos = transform.position;

            newPos.y = AnchorPoint.position.y + (int)Mathf.Round(RotationRadius * Mathf.Sin(angle));
            newPos.x = AnchorPoint.position.x + (int)Mathf.Round(RotationRadius * Mathf.Cos(angle));
            transform.position = newPos;
        }
    }

    
    public IEnumerator ShakeObject() // stop moving, grow object, and shake
    {
        Vector3 _startPos = transform.position;
        float _timer = 0;

        float shakeTime = 0.8f;
        float distance = 4f;

        canRotate = false;

        Vector3 prevSize = transform.localScale;
        transform.localScale *= 1.3f;
        while (_timer < shakeTime)
        {
            _timer += Time.deltaTime;

            Vector3 _randomPos = _startPos + (Random.insideUnitSphere * distance);

            transform.position = _randomPos;

            yield return null;
        }
        transform.localScale = prevSize;
        canRotate = true;
    }
    
}
