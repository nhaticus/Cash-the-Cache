using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullOpen : MonoBehaviour, InteractEvent
{
    [SerializeField] float movementRange = 0.2f;
    [SerializeField] float speed = 3f;
    Vector3 startPos;

    bool opened = false;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    public void Interact()
    {
        StartCoroutine(PullEvent());
    }

    private IEnumerator PullEvent()
    {
        opened = !opened;
        Vector3 currentPos = transform.localPosition;
        Vector3 targetPos;
        if (opened) // move target to new position
            targetPos = new Vector3(transform.localPosition.x, transform.localPosition.y, startPos.z + movementRange);
        else // closing: move back to start
            targetPos = startPos;
        //transform.localPosition = targetPos;
        float time = 0;
        while (time < 1)
        {
            transform.localPosition = Vector3.Lerp(currentPos, targetPos, time);
            time += Time.deltaTime * speed;
            yield return null;
        }
    }
}
