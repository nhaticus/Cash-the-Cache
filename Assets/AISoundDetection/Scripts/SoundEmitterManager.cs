using UnityEngine;
namespace AiSoundDetect
{
    [AddComponentMenu("AiSoundDetect/Sound Emitter Manager")]
	public class SoundEmitterManager : MonoBehaviour
    {
	    public AudioSource[] waveSources;
	    public AudioSource[] clipSources;
	    public int waveIndex;
	    public int clipIndex;
        
        [SerializeField]private SO_SoundManagerContainer SoundManagerContainer; 
	    void Start()
	    {
	    	SoundManagerContainer.SoundManager = this;
	    }
        public void SetWaveIndex()
        {
            if (waveIndex >= waveSources.Length - 1) waveIndex = 0;

        }
	    public void SetClipIndex()
	    {
		    if (clipIndex >= clipSources.Length - 1) clipIndex = 0;

	    }
    }
}
