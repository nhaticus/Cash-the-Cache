using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using AiSoundDetect.Extra;

namespace AiSoundDetect
{
public class StopAudio_Emitter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]private Sound_Emitter m_SoundEmitter;
	
	public enum eventType
	{
		PointerEnter, PointerExit,
		OnDisable, OnEnable, OnDestroy,
		OnStart,
		ColliderEnter,ColliderExit,
		ColliderNameEnter,ColliderNameExit,
		OnTagEnter,OnTagExit,
		OnClick
			
	};
	[Space(5)]
	[HideInInspector] public eventType typeEvent;
	[HideInInspector][TagSelector] public string TagFilter = "";
	[Tooltip("on self colider se isTrigger on -then Drag the gameObject that have a collider and rigidbody   ")]
	[HideInInspector]public GameObject m_ColliderObj;
	private Collider colliderTrigger;
	[HideInInspector]public string m_ColliderName = "nameOfObject";
	[HideInInspector] public Button clickButton;
	private bool hasQuitGame;
	#region OnPointerEnter
	public void OnPointerEnter(PointerEventData eventData) 
	{
		if(typeEvent == eventType.PointerEnter)
		{
			m_SoundEmitter.StopAudio();
		}
	}
		#endregion
	
	#region OnPointerExit
	public void OnPointerExit(PointerEventData eventData) 
	{
		if(typeEvent == eventType.PointerExit)
		{
			m_SoundEmitter.StopAudio();
		}
	}
		#endregion
	
   #region OnDisable
	void OnDisable()
	{
		if(typeEvent == eventType.OnDisable && Application.isPlaying)
		{
			m_SoundEmitter.StopAudio();
		}
	}
	#endregion	
	
	#region OnEnable
	void OnEnable()
	{
		if(typeEvent == eventType.OnEnable && Application.isPlaying)
		{
			m_SoundEmitter.StopAudio();
		}
	}
		#endregion
	
	#region Start
	void Start()
	{
		if(clickButton != null && typeEvent == eventType.OnClick )clickButton.onClick.AddListener(delegate{m_SoundEmitter.StopAudio();});
		if(typeEvent == eventType.OnStart && Application.isPlaying)
		{
			m_SoundEmitter.StopAudio();
		}
			
	}
	#endregion
	
	#region OnTriggerEnter
	private void OnTriggerEnter(Collider other)
	{
		if(typeEvent == eventType.ColliderEnter || typeEvent == eventType.ColliderNameEnter )
		{
			colliderTrigger = m_ColliderObj.GetComponent<Collider>();
		}
		if(typeEvent == eventType.ColliderEnter && colliderTrigger == other)
		{
			m_SoundEmitter.StopAudio();
		}
		if(typeEvent == eventType.ColliderNameEnter && colliderTrigger.name == m_ColliderName)
		{
			m_SoundEmitter.StopAudio();
		}
		if(typeEvent == eventType.OnTagEnter && other.tag == TagFilter)
		{
			m_SoundEmitter.StopAudio();
		}
	}
		#endregion
		
   #region OnTriggerExit
	private void OnTriggerExit(Collider other)
	{
		if(typeEvent == eventType.ColliderExit || typeEvent == eventType.ColliderNameExit)
		{
			colliderTrigger = m_ColliderObj.GetComponent<Collider>();
		}
		if(typeEvent == eventType.ColliderExit && colliderTrigger == other)
		{
			m_SoundEmitter.StopAudio();
		}
		if(typeEvent == eventType.ColliderNameExit && colliderTrigger.name == m_ColliderName)
		{
			m_SoundEmitter.StopAudio();
		}
		if(typeEvent == eventType.OnTagExit && other.tag == TagFilter)
		{
			m_SoundEmitter.StopAudio();
		}
	}
		#endregion		
	
	void OnApplicationQuit()
	{
		hasQuitGame = true;
	}	
	void OnDestroy()
	{
		if(typeEvent == eventType.OnDestroy && hasQuitGame == false ) { m_SoundEmitter.StopAudio();}
	}
}
}
