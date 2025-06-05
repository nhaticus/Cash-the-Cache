using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * A raycast attached to NPCs that looks for the player
 * When found, it increments a slider value
 * When slider value is full a PlayerRecognized event is emited
 * Does not control if the NPC stares at the player or runs away
 */

public class NPCDetection : MonoBehaviour
{
    /*  Detection  */
    [Header("Sight Range")]
    [SerializeField] float sightDistance;
    [SerializeField] int sightAngle; // Angle of the detection cone

    [SerializeField] float sightCountdown = 1.5f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    float sightTimer = 0.0f;

    [Header("Dependencies")]
    public DetectionBarController detectionBar;

    Transform player;

    public UnityEvent<GameObject> PlayerNoticed; // when player just touched detection
    public UnityEvent PlayerRecognized; // player stayed in detection for sightCountdown time
    public UnityEvent PlayerLost; // player just left detection
    bool playerStartUndetected = false;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        SendDetectionRaycast();
    }

    /// <summary>
    /// Send out a raycast that looks for the player
    /// </summary>
    void SendDetectionRaycast()
    {
        // find direction to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        bool objectDetected = Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightDistance); // fire raycast in direction of player
        
        // find if player is within sight cone
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        // check if within sight cone
        if (objectDetected && angleToPlayer <= sightAngle)
            CheckForPlayer(hit);
        else // object lost
            PlayerSightingLost();
    }
    
    /// <summary>
    /// Given a raycast hit, check if player is within range
    /// </summary>
    /// <param name="objectDetected"></param>
    void CheckForPlayer(RaycastHit objectDetected)
    {
        if (objectDetected.transform.CompareTag("Player")) // player tag checking
        {
            detectionBar.SetValue(sightTimer / sightCountdown);

            playerStartUndetected = false;
            PlayerNoticed.Invoke(objectDetected.transform.gameObject); // give game object so NPC can track position
            PlayerSightedBehavior();
        }
        else
            PlayerSightingLost();

    }

    /// <summary>
    /// Increasing sighting value
    /// When sighting value is full, emit PlayerRecognized event
    /// </summary>
    void PlayerSightedBehavior()
    {
        // increase sighting value
        sightTimer += Time.deltaTime;
        detectionBar.SetValue(sightTimer / sightCountdown);

        // if completed sighting: send event for NPC
        if (sightTimer >= sightCountdown)
        {
            CompleteDetection();
        }
    }

    /// <summary>
    /// Player was sighted then lost
    /// Send out PlayerLost event and decrease sighting value
    /// </summary>
    void PlayerSightingLost()
    {
        if (!playerStartUndetected) // object just lost, send 1 signal
        {
            PlayerLost.Invoke();
            playerStartUndetected = true;
        }

        sightTimer = Mathf.Max(0, sightTimer - Time.deltaTime);
        detectionBar.SetValue(sightTimer / sightCountdown);
    }

    public void CompleteDetection()
    {
        this.enabled = false; // turn self off
        PlayerRecognized.Invoke(); // send out event
        StartCoroutine(detectionBar.FlashingEffect()); // bar special effect
    }

    public void EmptyDetection()
    {
        sightTimer = 0;
        detectionBar.SetValue(sightTimer / sightCountdown);
    }

    private void OnDrawGizmosSelected()
    {
        // max detection range
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        // sight cone
        Vector3 leftLimit = Quaternion.Euler(0, -sightAngle, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, sightAngle, 0) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftLimit * sightDistance);
        Gizmos.DrawRay(transform.position, rightLimit * sightDistance);
    }

}
