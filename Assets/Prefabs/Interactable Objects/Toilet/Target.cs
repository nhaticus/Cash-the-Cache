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
    [SerializeField] float lowerBound, upperBound;

    [Header("Game Values")]
    [SerializeField] float timeToMove = 1;
    [SerializeField] float timeIncrease = 1.3f;
    float timer = 0;

    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timeToMove = 5 / canvas.difficulty;
    }

    public void OnTriggerStay2D(Collider2D Collision)
    {
        completionSlider.value += Time.deltaTime / (timeIncrease * canvas.difficulty);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToMove)
        {
            transform.localPosition = new Vector3(Random.Range(lowerBound, upperBound), 0, 0);

            timeToMove = Random.Range(1, 4f / canvas.difficulty);
            timer = 0;
        }
    }
}
