using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [SerializeField] FlushingCanvas canvas;
    [SerializeField] Slider powerSlida;
    float timer = 0;
    float timeToMove = 0;
    public bool SENDIT = false;
    public void OnTriggerStay(Collider Collision)
    {
        powerSlida.value += Time.deltaTime * 20 / canvas.difficulty;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeToMove)
        {
            timer = 0;
            timeToMove = Random.Range(1, 30 / canvas.difficulty);
            if (transform.GetComponent<RectTransform>().localPosition.x > 0)
            {
                transform.GetComponent<Rigidbody>().AddForce(new Vector3(-1 * Random.Range(100000, 1000000 * canvas.difficulty) * Time.deltaTime, 0f, 0f), ForceMode.Force);
            }
            else
            {
                transform.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(100000, 1000000 * canvas.difficulty) * Time.deltaTime, 0f, 0f), ForceMode.Force);
            }
        }
    }
}
