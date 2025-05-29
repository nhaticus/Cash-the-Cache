using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float mouseSens;
    public float controllerSens;

    public bool lockRotation = false;

    public Transform orientation;

    public float xRotation = 0;
    public float yRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mouseSens = PlayerPrefs.GetFloat("KeyboardCam Sensitivity", 120);
        controllerSens = PlayerPrefs.GetFloat("Controller Sensitivity", 120);
    }

    private void Update()
    {
        if (!lockRotation)
        {
            float camX = 0f;
            float camY = 0f;
            /*
            Vector2 stick = UserInput.Instance.Camera;
            camX = stick.x * Time.deltaTime * controllerSens;
            camY = stick.y * Time.deltaTime * controllerSens;
            */
            // Check for controller input otherwise use mouse sensitivity
            if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue() != Vector2.zero)
            {
                Vector2 stick = Gamepad.current.rightStick.ReadValue();
                camX = stick.x * Time.deltaTime * controllerSens;
                camY = stick.y * Time.deltaTime * controllerSens;
            }
            else
            {
                camX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSens;
                camY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSens;
            }
            
            yRotation += camX;
            xRotation -= camY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //rotate cam and orientaton
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
