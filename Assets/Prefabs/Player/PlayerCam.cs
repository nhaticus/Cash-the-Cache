using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sens;

    public bool lockRotation = false;

    public Transform orientation;

    public float xRotation;
    public float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!lockRotation)
        {
            float camX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            float camY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;
            
            yRotation += camX;
            xRotation -= camY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //rotate cam and orientaton
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }
}
