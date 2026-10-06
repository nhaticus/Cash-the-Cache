using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

/*
 * A raycast attached to NPCs that looks for the player
 * When found, it increments a slider value
 * When slider value is full a PlayerRecognized event is emited
 * Does not control if the NPC stares at the player or runs away
 */

public class NPCDetection : MonoBehaviour
{
    #region public variables

    /*  Detection  */
    [Header("Sight Range")]
    [SerializeField] float sightDistance;
    [SerializeField] int sightAngle; // Angle of the detection cone

    [Header("Sight Cooldown")]
    [SerializeField] float sightCountdown = 1.4f; // Time for how long the player needs to stay in line-of-sight before the enemy starts chasing
    [SerializeField] float minSightCountdown = 1f;
    [SerializeField] float sightMult = 1.03f;
    
    [Header("Dependencies")]
    public DetectionBarController detectionBar;

    #endregion

    #region private variables
    Transform player;

    public UnityEvent<GameObject> PlayerNoticed; // when player just touched detection
    public UnityEvent PlayerRecognized; // player stayed in detection for sightCountdown time
    public UnityEvent PlayerLost; // player just left detection
    bool playerStartUndetected = false;
    bool sendRaycast = true;
    float sightTimer = 0.0f;
    #endregion

    private void Start()
    {
        player = GameObject.Find("Player").transform;

        CalculateSightCountdown(PlayerPrefs.GetInt("Difficulty"));
    }

    private void Update()
    {
        // send raycast if not dead and player is active
        if (sendRaycast)
        {
            if ((PlayerManager.Instance == null) || (PlayerManager.Instance && PlayerManager.Instance.isPlayerActive))
                SendDetectionRaycast();
        } 
    }

    /// <summary>
    /// Only used in NPCBehavior
    /// Decreases sight countdown after knocked out
    /// </summary>
    /// <param name="multiplier"></param>
    public void DecreaseSightCountdown(float count)
    {
        sightCountdown -= count;
        sightCountdown = Mathf.Max(sightCountdown, minSightCountdown);
    }

    #region Private Functions

    /// <summary>
    /// alter sight countdown based on difficulty
    /// </summary>
    void CalculateSightCountdown(int difficulty)
    {
        sightCountdown /= difficulty * sightMult;
        sightCountdown = Mathf.Max(sightCountdown, minSightCountdown);
    }

    /// <summary>
    /// Send out a raycast that looks for the player
    /// </summary>
    void SendDetectionRaycast()
    {
        // find direction to player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer, sightDistance); // fire raycast in direction of player

        // SORT HITS: Nearest objects will now always be at index 0
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

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
        float playerDist = 10;
        float wallDist = 10;
        GameObject player = null;

        for (int i = 0; i < objectsDetected.Length; i++)
        {
            Transform currObject = objectsDetected[i].transform;
            if (currObject.CompareTag("Player")) // player tag checking
            {
                player = currObject.gameObject;
                float distance = objectsDetected[i].distance;
                if (distance < playerDist)
                    playerDist = distance;
            }
            else if (currObject.CompareTag("Wall") || currObject.gameObject.name.Contains("Door")) // prevent raycasting through wall
            {
                // check if distance is the closest wall
                float distance = objectsDetected[i].distance;
                if (distance < wallDist)
                    wallDist = distance;
            }
        }

        if (player && playerDist < wallDist)
        {
            detectionBar.SetValue(sightTimer / sightCountdown);

            playerStartUndetected = false;
            PlayerNoticed.Invoke(player); // give game object so NPC can track position
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
    #endregion
}
