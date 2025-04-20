using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FOKOzuynen
namespace AiSoundDetect.Extra
{
public class NotesAndInfo : MonoBehaviour
{
	
	
	[SerializeField] private string m_Title ;
	[Space(5)]
	[TextArea(15,20)]
	[SerializeField] private string Notes;
	[Space(10)]
	public GameObject[] objReference;
	
	
}
}
