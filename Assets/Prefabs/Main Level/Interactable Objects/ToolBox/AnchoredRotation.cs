using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Toolbox Minigame
 * Moves lock picker
 */

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

    float distance = 4f;
    public IEnumerator ShakeObject(float shakeTime) // stop moving, grow object, and shake
    {
        if (canRotate)
        {
            Vector3 _startPos = transform.position;
            canRotate = false;
            Vector3 prevSize = transform.localScale;
            transform.localScale *= 1.3f;

            float _timer = 0;
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

    public IEnumerator FreezeObject(float freezeTime)
    {
        if (canRotate)
        {
            canRotate = false;
            float _timer = 0;
            while (_timer < freezeTime)
            {
                _timer += Time.deltaTime;
                yield return null;
            }
            canRotate = true;
        }
    }
    
}
