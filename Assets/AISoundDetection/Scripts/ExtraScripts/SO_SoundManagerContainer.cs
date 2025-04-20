using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AiSoundDetect
{
[CreateAssetMenu(fileName = "ScriptableSoundManager", menuName= "ScriptableSoundManager")]
public class SO_SoundManagerContainer : ScriptableObject
{
	public SoundEmitterManager SoundManager;
}
}
