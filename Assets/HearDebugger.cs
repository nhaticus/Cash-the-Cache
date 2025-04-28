using UnityEngine;
public class HearDebugger : MonoBehaviour {
  void OnParticleCollision(GameObject other) {
    Debug.Log($"{name} GOT HIT by sound wave from {other.name}");
  }
}