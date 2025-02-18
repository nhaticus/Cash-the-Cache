using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TEST SCRIPTOBJ")]
public class TESTSCRIPTOBJ : ScriptableObject
{
    public GameObject gameObj;
    public int x;
}
public class BOOF : MonoBehaviour
{
    public TESTSCRIPTOBJ tESTSCRIPTOBJ;
}
