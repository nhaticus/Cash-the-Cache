using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * A raycast attached to NPCs that looks for the player
 * When found, it increments a slider value
 * When slider value is full a behavior can be called (Run, start police timer)
 * Does not control if the NPC stares at the player or runs away
 */

public class NPCDetection : MonoBehaviour
{
    /*  Detection  */
    [Header("Sight Range")]
    [SerializeField] float sightDistance;
    [SerializeField] int sightAngle; // Angle of the detection cone

    [Header("Behaviors")]
    [SerializeField] float cooldownMaxTime = 1.5f; // Time after losing sight of player to begin normal behavior
    float cooldownTimer = 0.0f;

    [SerializeField] float sightCountdown = 1.5f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    float sightTimer = 0.0f;

    [Header("Dependencies")]
    public Slider detectionSlider;

    Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        SendDetectionRaycast();

        if (playerStartUndetected)
            PlayerCheckUndetected();
        else if (playerEndUndetected)
            RewindDetection();
    }

    /// <summary>
    /// Send out a raycast that looks for the player
    /// </summary>
    void SendDetectionRaycast()
    {
        // find direction to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Debug.DrawRay(transform.position, directionToPlayer * sightDistance, Color.red);
        bool objectDetected = Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, sightDistance); // fire raycast in direction of player
        if(objectDetected)
            CheckForPlayer(hit);
        else if(!objectDetected && playerStartUndetected == false)
            PlayerSightingLost();
    }

    public Action<Vector3> PlayerNoticed;
    /// <summary>
    /// Given a raycast hit, check if player is within range
    /// </summary>
    /// <param name="objectDetected"></param>
    void CheckForPlayer(RaycastHit objectDetected)
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        
        if (angleToPlayer <= sightAngle)
        {
            if (objectDetected.transform.gameObject.CompareTag("Player")) // player checking
            {
                PlayerNoticed.Invoke(objectDetected.transform.position);
                PlayerSightedBehavior();
            }
        }
    }

    public Action PlayerRecognized;
    void PlayerSightedBehavior()
    {
        // increase sighting value
        sightTimer += Time.deltaTime;
        detectionSlider.value = sightTimer / sightCountdown;

        playerStartUndetected = false;

        // if completed sighting: send event for NPC
        if (sightTimer >= sightCountdown)
        {
            // turn self off
            this.enabled = false;
            PlayerRecognized.Invoke();
        }
    }

    bool playerStartUndetected = false;
    public Action PlayerStartLost;
    void PlayerSightingLost()
    {
        PlayerStartLost.Invoke();

        playerStartUndetected = true;
    }

    bool playerEndUndetected = false;
    public Action PlayerCompleteLost;
    void PlayerCheckUndetected()
    {
        cooldownTimer += Time.deltaTime; // stop and wait until cooldown
        Debug.Log(cooldownTimer);
        if (cooldownTimer >= cooldownMaxTime) // cooldown finished and continue normal behavior
        {
            playerStartUndetected = false;
            playerEndUndetected = true;
            PlayerCompleteLost.Invoke();
            cooldownTimer = 0;
        }
    }

    void RewindDetection()
    {
        Debug.Log("rewind");
        sightTimer = Mathf.Max(0, sightTimer - Time.deltaTime);
        detectionSlider.value = sightTimer / sightCountdown;
    }

    private void OnDrawGizmosSelected()
    {
        // max detection range
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, sightDistance);

        // range angle
        Vector3 leftLimit = Quaternion.Euler(0, -sightAngle, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, sightAngle, 0) * transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, leftLimit * sightDistance);
        Gizmos.DrawRay(transform.position, rightLimit * sightDistance);
    }

}
