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

    float angle = 0;

    bool canRotate = true;

    public void SetRotationSpeed(float speed) {
        this.speed = speed;
    }
    
    void Update()
    {
        if (canRotate)
        {
            Vector2 newPos = transform.position;
            newPos.x = AnchorPoint.position.x + Mathf.Cos(angle) * RotationRadius;
            newPos.y = AnchorPoint.position.y + Mathf.Sin(angle) * RotationRadius;
            transform.position = newPos;

            angle += speed * 1.4f * Time.deltaTime;
        }
    }

    float shakeTime = 0.75f;
    float distance = 4f;
    public IEnumerator ShakeObject() // stop moving, grow object, and shake
    {
        if (canRotate)
        {
            Vector3 _startPos = transform.position;
            float _timer = 0;

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
    
}
