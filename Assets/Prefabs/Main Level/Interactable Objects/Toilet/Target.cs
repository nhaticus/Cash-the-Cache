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
    [SerializeField] float minTimeToMove, maxTimeToMove;
    [SerializeField] float timeToMove = 1;
    [SerializeField] float timeIncrease = 1.3f;

    float timer = 0;

    float strength;

    private void Start()
    {
        // get strength and increase time to move
        strength = canvas.strength;
        minTimeToMove += strength * 0.15f;
        maxTimeToMove = (4 / canvas.difficulty) + (strength * 0.125f);

        // increase size based on strength
        transform.localScale += new Vector3(5 * strength, 0, 0);

        // set bounds to correct size
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
