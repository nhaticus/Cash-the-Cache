using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace AiSoundDetect.Extra
{
public class AImovement : MonoBehaviour
{
   
    [Tooltip("drag here object that contain AIHearing Script")]
    [SerializeField]
    private GameObject AIHearing;

    private bool soundDetectedGo;
    private Vector3 targetGo;
    [SerializeField]
    private bool chaseTarget = true; 
    NavMeshAgent navMeshAgent;
     public AudioSource AiVoice;
    void Start()
    {
       navMeshAgent = GetComponent<NavMeshAgent>();
       
    }

    void Update()
    {
      
        soundDetectedGo = AIHearing.GetComponent<AIHearing>().soundDetected;
       
         if ( soundDetectedGo && chaseTarget) 
         {  
           AiVoice.Play();
            targetGo = AIHearing.GetComponent<AIHearing>().targetObj;                              
	         if(targetGo != null) 
	         {
		         transform.LookAt(targetGo);  // AI will look at target
			     navMeshAgent.SetDestination(targetGo); //Ai will go to the position of sound
	         }
			}
        
		}
      
}
}

