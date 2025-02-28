using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sens = 50;
    // public float sensY;

    public bool lockRotation = false;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        sens = PlayerManager.Instance.mouseSensitivity;

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
