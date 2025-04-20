
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using UnityEditor.Audio;
using UnityEngine.Audio;

namespace AiSoundDetect
{
[CustomEditor (typeof(Sound_Emitter),true)]
public class EditorSoundEmitter : Editor
{
	[SerializeField] private SO_SoundManagerContainer SoundManagerContainer;
	[SerializeField] private AudioMixer clipMixer;
	[SerializeField] private AudioMixerGroup DepthMixer;
	[SerializeField] private AudioMixerGroup MasterMixer;
	[SerializeField] private AudioMixerSnapshot Near;
	[SerializeField] private AudioMixerSnapshot Far;
    public override void OnInspectorGUI()
	{
         
		base.OnInspectorGUI();
		Sound_Emitter script = (Sound_Emitter)target;
		script.clipMixer = clipMixer;
		script.DepthMixer = DepthMixer;
		script.MasterMixer = MasterMixer;
		script.Near = Near;
		script.Far = Far;
       if(SoundManagerContainer != null) script.soundEmitterManager = SoundManagerContainer.SoundManager;
		if(script.AudioMethod == Sound_Emitter.audioChoice.AudioSource) 
		{
			script.m_AudioClip = null;
			script.objectEmitterSource = EditorGUILayout.ObjectField("AudioSource",script.objectEmitterSource,typeof(AudioSource),true) as AudioSource;
			if(script.objectEmitterSource != null )
			{
				script.m_iAudioClip = script.objectEmitterSource.clip;
			}
		}
		else if(script.AudioMethod == Sound_Emitter.audioChoice.AudioClip) 
		{
			script.objectEmitterSource = null;
            
			script.m_AudioClip = EditorGUILayout.ObjectField("AudioClip",script.m_AudioClip,typeof(Object),true) as AudioClip;
			script.m_Volume =  EditorGUILayout.Slider("Volume",script.m_Volume,0f,1.0f);
			script.m_Pitch = EditorGUILayout.Slider("Pitch",script.m_Pitch,-3.0f,3.0f);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Spatial");
			GUILayout.Label("2D");
			script.m_SpatialBlend = EditorGUILayout.Slider(script.m_SpatialBlend,0f,1f);
			GUILayout.Label("3D");
			GUILayout.EndHorizontal();
			script.audioDistance = EditorGUILayout.FloatField("Distance",script.audioDistance);
			script.Trigger = (Sound_Emitter.eTrigger) EditorGUILayout.EnumPopup("Trigger",script.Trigger);
			if(script.Trigger == Sound_Emitter.eTrigger.ButtonClick)
			{
				script.clickButton =  EditorGUILayout.ObjectField ("This Button",script.clickButton,typeof (Button), true)as Button;
			}
			else if(script.Trigger == Sound_Emitter.eTrigger.OnCollisionName)
			{
				script.ColliderName = EditorGUILayout.TextField("Collider Name",script.ColliderName);
			}
			else if(script.Trigger == Sound_Emitter.eTrigger.OnCollisionTag)
			{
				script.ColliderTag = EditorGUILayout.TagField("Select Tag",script.ColliderTag);
			}
			else if(script.Trigger == Sound_Emitter.eTrigger.OnBool)
			{
				script.startMethod = EditorGUILayout.Toggle("If True",script.startMethod);
			}
			
			
			if(script.m_AudioClip != null )
			{
				script.m_iAudioClip = script.m_AudioClip;
			}
		}
		 
		//this is the part on Editor for audioclip inspection   
		GUILayout.BeginHorizontal();
		
		script.m_iAudioClip = EditorGUILayout.ObjectField(script.m_iAudioClip,typeof(Object),true) as AudioClip;
			
		if(script.AudioMethod == Sound_Emitter.audioChoice.AudioSource && GUILayout.Button("Play"))
		   { 
	            script.m_iAudioClip = script.objectEmitterSource.clip  ; //ADD a AudioSource if null references exeption
			SoundManagerContainer.SoundManager.clipSources[0].clip = script.m_iAudioClip ;
			SoundManagerContainer.SoundManager.clipSources[0].Play();
		   }
		else if(script.AudioMethod == Sound_Emitter.audioChoice.AudioClip && GUILayout.Button("Play"))
		{ 
			script.m_iAudioClip = script.m_AudioClip  ; 
			SoundManagerContainer.SoundManager.clipSources[0].clip = script.m_iAudioClip ;
			SoundManagerContainer.SoundManager.clipSources[0].Play();
		}
           if(GUILayout.Button("Stop"))
		   { 
	           SoundManagerContainer.SoundManager.clipSources[0].Stop();
          }
           
		if(script.m_iAudioClip != null)
			script.m_ClipLenght = EditorGUILayout.FloatField(Mathf.Round(script.m_iAudioClip.length  * 100f) / 100f);
		  GUILayout.EndHorizontal();
		  
	
	}
	
}
}
