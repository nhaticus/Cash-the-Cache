
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using AiSoundDetect.Extra;
//FOKOzuynen

namespace AiSoundDetect
{
	[AddComponentMenu("AiSoundDetect/AIHearing")]
public class AIHearing : MonoBehaviour
{
                                               public bool soundDetected  = false;  ///////this is the main Bool that should trigger any actions //////
	
	                         [SerializeField] private float perimeterAlert; 
	                            [TagSelector] public string TagFilter = "";
	[Tooltip("How loud to be the sound for to be detected")] 
	                          [SerializeField] private float soundDetectAroundCorner = 1.0f;
	[Tooltip("Set how far to hear around the corner")]
	                          [SerializeField] private float distanceHearingAroundCorner = 3.0f;
           [ReadOnlyInspector][SerializeField]private float distanceToObj = 0.0f;
	                          [SerializeField] private float highSoundLevel =30f;
	                          [SerializeField] private float timeAlert = 5.0f;
	       [ReadOnlyInspector][SerializeField] private float alertTime;
	                    [ReadOnlyInspector] public Vector3 targetObj ; //////// this is the Main target so the AI will focus on this///////
	     [HideInInspector][SerializeField]private Collider[] soundTargetObj; //all objects detected in perimeter
             [HideInInspector] [SerializeField]private bool targetDetected = false;
                                              private float soundEmitterLevel;
	                                           private float micSoundEmitterLevel;
	                                            private bool directSoundHit  = false;
                                        private NavMeshPath path;
                                       private NavMeshAgent NavAgent;
  [HideInInspector][SerializeField]private List<GameObject> ObjectFiltered;
	[HideInInspector][SerializeField]private List<GameObject> ObjectsCloseAroundCorner;
	 //public float rms;
	//public float dbv;
	//float preAlertTime =4f;
	//bool soundObstructed;
	 #region Start
    void Start()
    {
	    alertTime =timeAlert;
	    NavAgent = gameObject.GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
       soundTargetObj = new Collider[1];
	   
    }
	 #endregion
	 
    #region Update
    void Update()
    {
	   // SoundObstruction();
	    PerimeterScan();
      
	    if(targetDetected)
	    {
		   CalculateDistanceAroundCorner();
		   ActiveHearing();
	    	TimeAlert();
	    }
	   
	}
	  #endregion
	
	#region PerimeterScan
 public void PerimeterScan()  // this will check for objects how far can be listen and around corners
    {
       soundTargetObj = Physics.OverlapSphere(transform.position, perimeterAlert); 

	    if(distanceToObj > distanceHearingAroundCorner) {ObjectFiltered.Clear();ObjectsCloseAroundCorner.Clear();}
      foreach(var hit in soundTargetObj)
        {
          if(hit.CompareTag(TagFilter.ToString()))
	        {
	          if(!ObjectFiltered.Contains(hit.gameObject))
	          {ObjectFiltered.Add(hit.gameObject);}
	         
	          targetDetected = true;
	          //////Here we check if the sound is not to high and in that case the AI will notice indiferent of distance around the corner
	          
	          if(hit.gameObject.TryGetComponent <Sound_Emitter>(out Sound_Emitter componentSE) && componentSE.soundLevel >highSoundLevel  && !directSoundHit)// &&!soundObstructed)
	          {
	          
	             soundDetected = true;
		          targetObj = hit.transform.position;
	          }
            else if(hit.gameObject.TryGetComponent <Mic_Emitter>(out Mic_Emitter componentMic)&& componentMic.micSoundLevel>highSoundLevel && !directSoundHit)//&&!soundObstructed)
	          { 
	          	soundDetected = true;
		          targetObj = hit.transform.position;    
	          }
            }
	      }
    }
    #endregion
    
	#region ActiveHearing
	void ActiveHearing()
	{
      foreach(GameObject obj in ObjectsCloseAroundCorner) 
		{
			if(obj == null){ObjectFiltered.Clear();ObjectsCloseAroundCorner.Clear();}
	      
	      if(obj.GetComponent<Sound_Emitter>()
				&& !directSoundHit
				&& obj.GetComponent<Sound_Emitter>().soundLevel > soundDetectAroundCorner) //detects sounds around the corner
				{
					soundDetected = true;
		      targetObj = obj.transform.position;
		      Debug.Log("I can hear you around the corner");  // delete this line for not having console blown
				}
	      else if(obj.GetComponent<Mic_Emitter>()  // detect mic sounds around the corner
				&& !directSoundHit
				&& obj.GetComponent<Mic_Emitter>().micSoundLevel > soundDetectAroundCorner)
			   {
				   soundDetected = true;
		      targetObj = obj.transform.position;
		      Debug.Log("I can hear your Voice around the corner");  // delete this line for not having console blown
			   }
      }
		
	}
	#endregion
	
	#region CalculateDistanceAroundCorner
	void CalculateDistanceAroundCorner()
	{
		foreach(GameObject Obj in ObjectFiltered) 
		{
			if(Obj == null) {ObjectFiltered.Clear();ObjectsCloseAroundCorner.Clear();}
			
			if( !directSoundHit && Obj != null 
			  && NavMesh.CalculatePath(transform.position, Obj.transform.position, NavAgent.areaMask, path)) // need to be a path else cant calculate a distance to nowhere
			{
				
				distanceToObj = Vector3.Distance(transform.position, path.corners[0]);  //remember to add navmesh surface and bake it
				if(path.corners.Length>2)
				{
					for(int c = 1;c < path.corners.Length; c++)
					{
						//Debug.DrawLine(path.corners[c-1], path.corners[c], Color.red);  // this line provide visual in scene 
						distanceToObj += Vector3.Distance(path.corners[c-1], path.corners[c]);
					}
					if(distanceToObj < distanceHearingAroundCorner)
					{
						if(!ObjectsCloseAroundCorner.Contains(Obj))
						ObjectsCloseAroundCorner.Add(Obj);
					}
				
			    }
			}
			
		}
	}
    #endregion
    
    #region TimeAlert
	public void TimeAlert()
    {
       if(soundDetected)
       {
           alertTime -= Time.deltaTime;
           if(alertTime < 0)
           {
            targetDetected = false;
            alertTime = timeAlert;
            soundDetected = false; 
            //targetObj = null;
            for(int i = 0; i< soundTargetObj.Length;i++)
            {
            	soundTargetObj[i]= null;
            }
            directSoundHit=false;
           }
       }
    }
	#endregion
	
	#region OnParticleCollision
       void OnParticleCollision(GameObject other)   // this is on direct sound collision
       { 
	       
	       if(other.tag == TagFilter.ToString() ) 
          {
            targetDetected =true;
          	directSoundHit = true;
            soundDetected = true;
            Debug.Log("I can hear YOU!!!");
		       
            if(alertTime == timeAlert)
            {
	           targetObj = other.gameObject.transform.position;

	         }
          }
          
       }
      #endregion 
      
      /*
	void SoundObstruction() // work in progress - sound can be obstructed by other sounds
	{
		const int QSAMPLES = 128;
		const float REFVAL = 0.1f;  // RMS for 0 dB
 
		float[] samples = new float[QSAMPLES];
 
		AudioListener.GetOutputData(samples, 0);
 
		float sqrSum = 0.0f;
 
		int i = QSAMPLES; while (i --> 0) {
 
			sqrSum += samples[i] * samples[i];
		}
 
		rms = Mathf.Sqrt(sqrSum/QSAMPLES); // rms value 0-1
		//dbv = 20.0f*Mathf.Log10(rms/REFVAL); // dB value
		dbv = 20.0f*Mathf.Abs(rms/REFVAL); // dB value
		if( dbv > 40 )
		{
			soundObstructed = true;
		}
		if(soundObstructed == true)
		{
			preAlertTime -= Time.deltaTime;
			if(preAlertTime<= 0)
			{
				soundObstructed = false;
				preAlertTime = 2f;
			}
		}
		
	}  */
	
	#region OnDrawGizmosSelected
       void OnDrawGizmosSelected()   // visual perimeter in scene of AI 
      {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, perimeterAlert );
      }
      #endregion
}  
}
     


