using UnityEngine;
using UnityEngine.AI;

public class NPCFootsteps : MonoBehaviour
{
    public string footstepClipName = "footstep_sound";
    private AudioSource audioSource;
    private NavMeshAgent agent;
    private bool isPlayingFootsteps = false;
    public float velocityThreshold = 0.1f; 



    void Awake()
    {
        // Ensure the NPC has an AudioSource configured for 3D sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f; // Full 3D sound
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        audioSource.minDistance = 2f;  // max volume at 2 units
        audioSource.maxDistance = 8f;  // fades from 8
        audioSource.rolloffMode = AudioRolloffMode.Linear; // Use linear falloff for consistent reduction

        // Get the NavMeshAgent 
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError("NPCFootsteps requires a NavMeshAgent component.");
    }

    void Update()
    {
        if (Time.timeScale <= 0) // Prevent sound when paused
        {
            StopFootsteps();
            return;
        }

        // Check if NPC is moving based on NavMeshAgent's velocity magnitude
        bool isMoving = agent.velocity.sqrMagnitude > velocityThreshold * velocityThreshold;
        if (isMoving && !isPlayingFootsteps)
        {
            PlayFootsteps();
        }
        else if (!isMoving && isPlayingFootsteps)
        {
            StopFootsteps();
        }
    }

    private void PlayFootsteps()
    {
        
        Sound s = System.Array.Find(AudioManager.Instance.sfxSounds, sound => sound.name == footstepClipName);
        if (s != null)
        {
            audioSource.clip = s.clip;
            audioSource.Play();
            isPlayingFootsteps = true;
        }
        else
        {
            Debug.LogWarning("Footstep sound not found in AudioManager.");
        }
    }

    private void StopFootsteps()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        isPlayingFootsteps = false;
    }
}
