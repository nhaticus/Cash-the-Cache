using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Controls the random movement and slider increment for colliding the green and black box.
 */
public class Target : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] FlushingCanvas canvas;
    [SerializeField] Slider completionSlider;
    [SerializeField] float minForce = 100000, maxForce = 1000000;
    [SerializeField] float lowerBound, upperBound;

    [Header("Game Values")]
    [SerializeField] float timeToMove = 1;
    [SerializeField] float sliderIncrease = 1.1f;
    float timer = 0;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timeToMove = 5 / canvas.difficulty;
    }

    public void OnTriggerStay2D(Collider2D Collision)
    {
        completionSlider.value += Time.deltaTime / (sliderIncrease * canvas.difficulty);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToMove)
        {
            if (transform.GetComponent<RectTransform>().localPosition.x > 0)
                rb.AddForce(new Vector2(-1 * Random.Range(minForce, maxForce * canvas.difficulty) * Time.deltaTime, 0f), ForceMode2D.Force);
            else
                rb.AddForce(new Vector2(Random.Range(minForce, maxForce * canvas.difficulty) * Time.deltaTime, 0f), ForceMode2D.Force);

            timeToMove = Random.Range(1, 4f / canvas.difficulty);
            timer = 0;
        }

        if (transform.localPosition.x < lowerBound || transform.localPosition.x > upperBound)
            rb.velocity = Vector2.zero;
    }
}
