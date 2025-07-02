using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    bool sendRaycast = true;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if(sendRaycast)
            SendDetectionRaycast();
    }

    /// <summary>
    /// Send out a raycast that looks for the player
    /// </summary>
    void SendDetectionRaycast()
    {
        // find direction to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer, sightDistance); // fire raycast in direction of player
        
        // find if player is within sight cone
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (hits.Length > 0 && angleToPlayer <= sightAngle)
            CheckForPlayer(hits);
        else // object lost
            PlayerSightingLost();
    }

    /// <summary>
    /// Given raycast hit, check if player is in objects found
    /// </summary>
    /// <param name="objectDetected"></param>
    void CheckForPlayer(RaycastHit[] objectsDetected)
    {
        for (int i = 0; i < objectsDetected.Length; i++)
        {
            if (objectsDetected[i].transform.CompareTag("Player")) // player tag checking
            {
                detectionBar.SetValue(sightTimer / sightCountdown);

                playerStartUndetected = false;
                PlayerNoticed.Invoke(objectsDetected[i].transform.gameObject); // give game object so NPC can track position
                PlayerSightedBehavior();
                return;
            }
        }

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
        sendRaycast = false; // stop looking for player
        PlayerRecognized.Invoke(); // send out event
        StartCoroutine(detectionBar.FlashingEffect()); // bar special effect
    }

    // happens when NPC is knocked out
    public void EmptyDetection()
    {
        sendRaycast = true; // reset looking for player
        sightTimer = 0; // empty out sight bar
        detectionBar.SetValue(0);
    }

    private void OnDrawGizmosSelected()
    {
        // max detection range
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        // sight cone
        Gizmos.color = Color.yellow;
        Vector3 leftLimit = Quaternion.Euler(0, -sightAngle, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, sightAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, leftLimit * sightDistance);
        Gizmos.DrawRay(transform.position, rightLimit * sightDistance);
    }

}
