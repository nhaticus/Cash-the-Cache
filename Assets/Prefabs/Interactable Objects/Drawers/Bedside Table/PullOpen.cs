using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PullOpen : MonoBehaviour, InteractEvent
{
    [SerializeField] float movementRange = 0.2f;
    [SerializeField] float speed = 3f;
    public Vector3 startPos;

    Vector3 currentPos, targetPos;

    bool opened = false;
    bool pull = false;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        if(pull)
            transform.position = Vector3.Lerp(currentPos, targetPos, Time.time);
    }

    public void Interact()
    {
        PullEvent();
    }

    private void PullEvent()
    {
        opened = !opened;
        currentPos = transform.position;
        targetPos = opened ? new Vector3(transform.position.x, transform.position.y, startPos.z + movementRange) : startPos;
        pull = true;
        //targetPos = opened ? startPos + movementRange : startPos;
        /*
        while (Mathf.Abs(Mathf.DeltaAngle(currentPos, targetPos)) > 0.1f)
        {
            currentPos = Mathf.Lerp(currentPos, targetPos, Time.deltaTime * speed);
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, currentPos);
            yield return null;
        }*/
    }
}
