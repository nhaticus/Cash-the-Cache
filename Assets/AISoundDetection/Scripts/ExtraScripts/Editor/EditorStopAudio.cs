using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace AiSoundDetect
{
	[CustomEditor (typeof(StopAudio_Emitter),true)]
	public class EditorStopAudio : Editor
{
    
	public override void OnInspectorGUI()
	{
         
		base.OnInspectorGUI();
		StopAudio_Emitter script = (StopAudio_Emitter)target;
    
		script.typeEvent = (StopAudio_Emitter.eventType)EditorGUILayout.EnumPopup("Type Event",script.typeEvent);
		if(script.typeEvent == StopAudio_Emitter.eventType.ColliderNameExit)
		{
			script.m_ColliderName = EditorGUILayout.TextField("Collider Name",script.m_ColliderName);
		}
		else if(script.typeEvent == StopAudio_Emitter.eventType.ColliderNameEnter)
		{
			script.m_ColliderName = EditorGUILayout.TextField("Collider Name",script.m_ColliderName);
		}
		else if(script.typeEvent == StopAudio_Emitter.eventType.OnClick)
		{
			script.clickButton =  EditorGUILayout.ObjectField ("This Button",script.clickButton,typeof (Button), true)as Button;
		}
		else if(script.typeEvent == StopAudio_Emitter.eventType.ColliderEnter)
		{
			script.m_ColliderObj = EditorGUILayout.ObjectField("ColliderObj",script.m_ColliderObj,typeof (GameObject), true) as GameObject;
		}
		else if(script.typeEvent == StopAudio_Emitter.eventType.ColliderExit)
		{
			script.m_ColliderObj = EditorGUILayout.ObjectField("ColliderObj",script.m_ColliderObj,typeof (GameObject), true) as GameObject;
		}
		else if(script.typeEvent == StopAudio_Emitter.eventType.OnTagEnter)
		{
			script.TagFilter = EditorGUILayout.TagField("Select Tag",script.TagFilter);
		}
		else if(script.typeEvent == StopAudio_Emitter.eventType.OnTagExit)
		{
			script.TagFilter = EditorGUILayout.TagField("Select Tag",script.TagFilter);
		}
    
    
	}
    
}
}
