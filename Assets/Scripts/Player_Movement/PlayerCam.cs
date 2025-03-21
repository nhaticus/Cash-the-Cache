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

        sens = PlayerPrefs.GetFloat("KeyboardCam Sensitivity", 120);
    }

    private void Update()
    {
        if (!lockRotation)
        {
            // get mouse input
            float camX = UserInput.Instance.CameraInput.x * Time.deltaTime * sens;
            float camY = UserInput.Instance.CameraInput.y * Time.deltaTime * sens;

            yRotation += camX;
            xRotation -= camY;


            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //rotate cam and orientaton
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }
}
