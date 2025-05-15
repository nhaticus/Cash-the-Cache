using UnityEngine;

public class DestroyOnInteract : MonoBehaviour, InteractEvent
{
    public void Interact()
    {
        Destroy(gameObject);
    }
}
