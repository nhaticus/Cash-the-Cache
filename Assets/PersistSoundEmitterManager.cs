using UnityEngine;
namespace AiSoundDetect {
  public class PersistSoundEmitterManager : MonoBehaviour {
    void Awake() {
      DontDestroyOnLoad(gameObject);
    }
  }
}
