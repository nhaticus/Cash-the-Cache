using UnityEngine;

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
            // get mouse input
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;


            yRotation += mouseX;
            xRotation -= mouseY;


            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //rotate cam and orientaton
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }
}
