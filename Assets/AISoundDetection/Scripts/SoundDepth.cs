using UnityEngine;
using System.Collections;
using AiSoundDetect.Extra;

namespace AiSoundDetect
{
	[AddComponentMenu("AiSoundDetect/Sound Depth")]	
	public class SoundDepth : MonoBehaviour
{
	private float weight = 1;
	private float[] weightResult;
	[SerializeField] private float weightTune = 2.6f;	
	[SerializeField] private Transform Player;
	 private bool seeTarghet;
	[SerializeField] private LayerMask layerToCollideWith;
	[SerializeField] private float range = 15f;
	[ReadOnlyInspector][SerializeField] private float distance;
	[SerializeField] private Sound_Emitter m_SoundEmitter;
	
	void Update()
	{
			
		distance = Vector3.Distance(transform.position,Player.position);
		
		if(distance <= range)
		{
			LineCast();
		if(!seeTarghet)
		{
			if(distance >2)weight = 1/distance*weightTune;
			if(distance <2) weight = 0.9f;
		}
		if(seeTarghet)weight = 1f;
			
		weightResult = new float[2] { weight, 1.0f - weight };
		m_SoundEmitter.weightResult = weightResult;	
		}
	}
		
	void LineCast()
	{
		if (Physics.Linecast(transform.position, Player.position,layerToCollideWith))
		{
			seeTarghet = false;
		}
		else seeTarghet = true;
	}
	void OnDrawGizmosSelected()   // visual perimeter in scene of range
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, range );
	}		
		
}
}