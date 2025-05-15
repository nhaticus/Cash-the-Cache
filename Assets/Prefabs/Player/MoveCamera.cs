using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
            transform.position = new Vector3(cameraPosition.position.x, transform.position.y, cameraPosition.position.z);
    }
}
