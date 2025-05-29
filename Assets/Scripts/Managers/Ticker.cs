using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    public float totalTickTime = 0.2f;
    public float totalSlowTickTime = 0.75f;

    float tickTimer;
    float slowTickTimer;

    public delegate void TickAction();
    public static event TickAction OnTickAction;

    public delegate void SlowTickAction();
    public static event SlowTickAction OnSlowTickAction;

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if(tickTimer >= totalTickTime)
        {
            OnTickAction?.Invoke();
            tickTimer = 0;
        }

        slowTickTimer += Time.deltaTime;
        if (slowTickTimer >= totalSlowTickTime)
        {
            OnSlowTickAction?.Invoke();
            tickTimer = 0;
        }
    }

}
