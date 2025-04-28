using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * controls the random movement and slider increment for colliding the green and black box.
 */
public class Target : MonoBehaviour
{
    [SerializeField] FlushingCanvas canvas;
    [SerializeField] Slider powerSlida;
    [SerializeField] float minForce = 100000, maxForce = 1000000;
    float timer = 0;
    float timeToMove = 0;
    public bool SENDIT = false;
    public void OnTriggerStay(Collider Collision)
    {
        powerSlida.value += Time.deltaTime * (2 / canvas.difficulty);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeToMove)
        {
            if (transform.GetComponent<RectTransform>().localPosition.x > 0)
            {
                transform.GetComponent<Rigidbody>().AddForce(new Vector2(-1 * Random.Range(minForce, maxForce * canvas.difficulty) * Time.deltaTime, 0f), ForceMode.Force);
            }
            else
            {
                transform.GetComponent<Rigidbody>().AddForce(new Vector2(Random.Range(minForce, maxForce * canvas.difficulty) * Time.deltaTime, 0f), ForceMode.Force);
            }
            timeToMove = Random.Range(1, 30 / canvas.difficulty);
            timer = 0;
        }
    }
}
