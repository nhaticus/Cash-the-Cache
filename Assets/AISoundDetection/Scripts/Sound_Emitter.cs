using UnityEngine;
using UnityEngine.Audio;
using System;
using AiSoundDetect.Extra;
using UnityEngine.UI;
//FOKOzuynen
namespace AiSoundDetect
{
	[AddComponentMenu("AiSoundDetect/Sound_Emiter")]
	public class Sound_Emitter : MonoBehaviour
	{
		//[SerializeField]SoundDepth m_SoundDepth;
		[HideInInspector]public float[] weightResult;
		[HideInInspector]public AudioMixer clipMixer;
		[HideInInspector]public AudioMixerGroup DepthMixer;
		[HideInInspector]public AudioMixerGroup MasterMixer;
		[HideInInspector]public AudioMixerSnapshot Near;
		[HideInInspector]public AudioMixerSnapshot Far;
		AudioMixerSnapshot[] AMS;
		[Tooltip("Drag here the AudioSource that emitt sound")]
		[HideInInspector][SerializeField] public AudioSource objectEmitterSource;
		[HideInInspector][SerializeField] public AudioClip m_AudioClip;
		[HideInInspector]public  AudioSource waveEmitter;
		private AudioMixerGroup mixerEmitter;
		[Tooltip("Set the Distance the wave will travel")]
		[SerializeField][Range(0.0f,10.0f)] private float WaveDistance = 3f; 
		[SerializeField][ReadOnlyInspector]public float soundLevel ;
		[SerializeField][ReadOnlyInspector]public float maxSoundLevelReach;
		[SerializeField][Range(0.0f,1000.0f)]private float soundDensity = 300.0f;
		private float soundLevelScale = 10f;
	 const int QSAMPLES = 128;
	 const float REFVAL = 0.1f;  // RMS for 0 dB
		private float[] samples;
		[SerializeField] private LayerMask layerToCollideWith;
		[SerializeField] private bool enableSoundVisualization = true;  // this will hide sound hits
		[HideInInspector]public ParticleSystem m_particleSystem;
		[HideInInspector]public AudioClip m_iAudioClip;
		[HideInInspector]public float m_ClipLenght;
		private float m_xClipLenght;
		[HideInInspector] public SoundEmitterManager soundEmitterManager;
		private bool clipIsPlaying ;
	
		public enum audioChoice
		{
			AudioSource, AudioClip
		}
		public audioChoice AudioMethod;
		[HideInInspector] public float m_Volume = 1f;
		[HideInInspector] public float m_Pitch = 1f; 
		[HideInInspector] public float m_SpatialBlend = 1f; 
		[Tooltip("Min distance")]
		[HideInInspector]public float audioDistance = 0.2f;
		[HideInInspector]public enum eTrigger
		{ButtonClick, OnCollisionName, OnCollisionTag, OnEnable, OnBool }
		[HideInInspector]public eTrigger Trigger;
		[HideInInspector] public string ColliderName;
		[HideInInspector] public Button clickButton;
		[HideInInspector] public bool startMethod;
        [Tooltip("the colider has to be with isTrigger On and if collidider have problems read documentation")]
		[HideInInspector][TagSelector] public string ColliderTag = "";
		private bool findAudio ;
		private bool clipStart ;
		private int ClipIndexContainer;
		void Start()
		{
			if(clickButton != null && Trigger == eTrigger.ButtonClick )clickButton.onClick.AddListener(delegate{startMethod=true;});
		}
		
	   void OnEnable()
		{
			AMS =  new AudioMixerSnapshot[2] { Near, Far};
			if(soundEmitterManager== null)
			soundEmitterManager = FindObjectOfType<SoundEmitterManager>();
			waveEmitter = soundEmitterManager.waveSources[ soundEmitterManager.waveIndex];  
			SetParticleSystem();
       
			if(Trigger == eTrigger.OnEnable)
			{startMethod = true;}
			if(AudioMethod == audioChoice.AudioClip)	
				m_xClipLenght = m_AudioClip.length/m_Pitch;
             if(AudioMethod == audioChoice.AudioSource)
				m_xClipLenght = objectEmitterSource.clip.length/objectEmitterSource.pitch;
	   }
    
		public void SetParticleSystem()
		{
			//adding a Particle System and set it up.
			if(gameObject.GetComponent<ParticleSystem>() == null)
			{
				m_particleSystem = gameObject.AddComponent(typeof(ParticleSystem)) as ParticleSystem;
			}
			else m_particleSystem = gameObject.GetComponent<ParticleSystem>();
			m_particleSystem.Stop();
			var particleMain = m_particleSystem.main;
			particleMain.duration = 2f;
			particleMain.startSize = 0.1f;
			particleMain.startSpeed = 15f;
			var emission = m_particleSystem.emission;
			emission.rateOverTime = soundDensity;
			var shape = m_particleSystem.shape;
			shape.enabled = true;
			shape.shapeType = ParticleSystemShapeType.Sphere;
			var collision = m_particleSystem.collision;
			collision.enabled = true;
			collision.bounce = 10f;
			collision.collidesWith = layerToCollideWith;
			collision.type = ParticleSystemCollisionType.World;
			collision.sendCollisionMessages = true;
			var renderer = m_particleSystem.GetComponent<ParticleSystemRenderer>();
			renderer.enabled = enableSoundVisualization;
		}
	
		void Update()
		{
			if(AudioMethod == audioChoice.AudioSource) 
			 SoundSourceProcess(); 
			
			 if(AudioMethod == audioChoice.AudioClip && startMethod)
			{
				findAudio = true;
				clipStart = true;
				startMethod = false;
			}
			if(AudioMethod != audioChoice.AudioSource)
			   SoundClipProcess(); //have to stay outside else is stoped by bool startMethod
		}
		public void ClipPlay() //method that can be call from custom event
		{
			startMethod = true;
		}
		public void StopAudio()
		{
			if(AudioMethod == audioChoice.AudioClip)
			{
				soundEmitterManager.clipSources[ClipIndexContainer].Stop();
				soundEmitterManager.waveSources[ClipIndexContainer].Stop();
				clipIsPlaying = false;
				clipStart = false;
			}
			else if(AudioMethod == audioChoice.AudioSource)
			{
				clipIsPlaying = false;
				findAudio = false;
				soundEmitterManager.waveSources[ClipIndexContainer].Stop();	
				objectEmitterSource.Stop();
			}
			
			
		}
		void SoundClipProcess()
		{
			if(findAudio)
			{
				soundEmitterManager.clipIndex =  soundEmitterManager.clipIndex +1;
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].clip = m_AudioClip;
				ClipIndexContainer = soundEmitterManager.clipIndex;
				/// send our clip config
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].volume = m_Volume;
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].pitch = m_Pitch;
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].spatialBlend = m_SpatialBlend;
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].minDistance = audioDistance;
				
				if(weightResult.Length >0 && weightResult[0]<1)
				{
					soundEmitterManager.clipSources[soundEmitterManager.clipIndex].outputAudioMixerGroup = DepthMixer;
					soundEmitterManager.clipSources[soundEmitterManager.clipIndex].outputAudioMixerGroup.audioMixer.TransitionToSnapshots(AMS,weightResult, 0.3f);
				}
				else if(weightResult.Length < 0 || weightResult.Length >0 && weightResult[0]>=1)
				{
					soundEmitterManager.clipSources[soundEmitterManager.clipIndex].outputAudioMixerGroup = MasterMixer;
				}
				// we set the audio position on our gameobject position
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].gameObject.transform.localPosition = gameObject.transform.position; 
				soundEmitterManager.clipSources[soundEmitterManager.clipIndex].Play();       
				soundEmitterManager.SetClipIndex();  
				soundEmitterManager.waveIndex  = soundEmitterManager.waveIndex +1;
				waveEmitter = soundEmitterManager.waveSources[soundEmitterManager.waveIndex] ;
				soundEmitterManager.SetWaveIndex();
			      
				findAudio = false;
			}	
		
			if(clipStart)
			{
				m_xClipLenght -= Time.deltaTime;
				if(m_xClipLenght <=0)
				{
					m_xClipLenght = m_AudioClip.length/m_Pitch;
					clipStart =false;
				}
				SoundAnalyzer(m_AudioClip);
				if(soundLevel > maxSoundLevelReach)
				{
					maxSoundLevelReach = soundLevel;
				}
			}
			else if(!clipStart)
			{
				soundLevel = 0;
				m_particleSystem.Stop();
			}
		}
		
  void SoundSourceProcess()
		{
			if( objectEmitterSource.isPlaying && !findAudio)
			{
				   soundEmitterManager.waveIndex  = soundEmitterManager.waveIndex +1;
					waveEmitter = soundEmitterManager.waveSources[soundEmitterManager.waveIndex] ;
					soundEmitterManager.SetWaveIndex();
					clipIsPlaying = true;
					findAudio = true;
			}
			 if(clipIsPlaying)
			{
				m_xClipLenght -= Time.deltaTime;
				if(m_xClipLenght <=0)
				{
					m_xClipLenght = objectEmitterSource.clip.length/objectEmitterSource.pitch;
					findAudio = false;
					clipIsPlaying =false;
				}
				SoundAnalyzer(objectEmitterSource.clip);
				if(soundLevel > maxSoundLevelReach)
				{
					maxSoundLevelReach = soundLevel;
				}
			}
            else if(!clipIsPlaying)
            {
                soundLevel = 0; // if audioclip is stoped before finish soundLevel will remain at last level sound.
				m_particleSystem.Stop();
            }
	   }
	
		public void SoundAnalyzer(AudioClip aClip)
		{
		
			if( !waveEmitter.isPlaying ) 
			{
				waveEmitter.clip = aClip;
				waveEmitter.Play();
			}
			samples = new float[QSAMPLES];
			waveEmitter.GetOutputData(samples, 0);
			float sqrSum = 0.0f;
			int i = QSAMPLES;
			while (i --> 0)
			{
				sqrSum += samples[i] * samples[i];
			}
 
			soundLevelScale =Mathf.Sqrt(sqrSum/QSAMPLES); 
			soundLevel = 20.0f*Mathf.Abs(soundLevelScale/REFVAL); // dB value
			////////////////////////////////////////////////////////////
			if(soundLevel <= 0.33 ) // 0.33 is minimum silence (yeah even silence have a sound)
			{
				gameObject.GetComponent<ParticleSystem>().Stop();
			}
			if(soundLevel >= 0.5 )
			{
				//here the sound wave particle travel distance in realtime
				var particleMains = gameObject. GetComponent<ParticleSystem>().main;
				particleMains.startLifetime = (this.soundLevel/100)*WaveDistance;
				gameObject.GetComponent<ParticleSystem>().Play();
			}
		}
  
		void OnTriggerEnter(Collider col)
		{
			if(AudioMethod == audioChoice.AudioClip)
			{
			if( col.gameObject.name == ColliderName)
			{startMethod = true;}
			if(Trigger == eTrigger.OnCollisionTag && col.gameObject.tag == ColliderTag)
			{startMethod = true;}
			}
		}
	}
}

