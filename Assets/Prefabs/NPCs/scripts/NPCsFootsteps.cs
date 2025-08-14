using UnityEngine;
using UnityEngine.AI;

public class NPCFootsteps : MonoBehaviour
{
    [SerializeField] SingleAudio singleAudio;
    public string footstepClipName = "footsteps";

    private NavMeshAgent agent;
    private bool isPlayingFootsteps = false;
    public float velocityThreshold = 0.1f; 

    void Awake()
    {
        // set all sfx sources to spatial audio
        foreach (AudioSource source in singleAudio.sfxSources)
        {
            source.spatialBlend = 1f; // Full 3D sound
            source.loop = true;
            source.playOnAwake = false;

            source.minDistance = 2f;  // max volume at 2 units
            source.maxDistance = 8f;  // fades from 8
            source.rolloffMode = AudioRolloffMode.Linear; // Use linear falloff for consistent reduction
        }

        // create new source
        if (singleAudio.sfxSources.Length == 0)
        {
            AudioSource sfxSource = singleAudio.gameObject.AddComponent<AudioSource>();
            sfxSource.spatialBlend = 1f; // Full 3D sound
            sfxSource.loop = true;
            sfxSource.playOnAwake = false;

            sfxSource.minDistance = 2f;  // max volume at 2 units
            sfxSource.maxDistance = 8f;  // fades from 8
            sfxSource.rolloffMode = AudioRolloffMode.Linear; // Use linear falloff for consistent reduction
        }

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
        singleAudio.PlaySFX(footstepClipName, singleAudio.sfxSources[0]);
        isPlayingFootsteps = true;
    }

    private void StopFootsteps()
    {
        singleAudio.StopSelectSFX("footsteps", singleAudio.sfxSources[0]);
        isPlayingFootsteps = false;
    }
}
