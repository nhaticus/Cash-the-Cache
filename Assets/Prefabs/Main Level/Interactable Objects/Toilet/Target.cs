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
    [SerializeField] float minTimeToMove = 1.5f, maxTimeToMove = 3.5f;
    [SerializeField] float timeToMove = 1;
    [SerializeField] float timeIncrease = 1.3f;

    float timer = 0;

    private void Start()
    {
        // get strength and increase time to move
        float strength = canvas.strength;
        minTimeToMove -= canvas.difficulty * 0.065f;
        minTimeToMove += strength * 0.13f;

        maxTimeToMove -= canvas.difficulty * 0.175f;
        maxTimeToMove += strength * 0.12f;

        // increase size based on strength
        transform.localScale += new Vector3(3 * strength, 0, 0);

        // set bounds based on adjusted size
        float halfScaleX = transform.localScale.x * 0.5f;
        lowerBound += halfScaleX;
        upperBound -= halfScaleX;
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

            timeToMove = Random.Range(minTimeToMove, maxTimeToMove);
            timer = 0;
        }
    }
}
