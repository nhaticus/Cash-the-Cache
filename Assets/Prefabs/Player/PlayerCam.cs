using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sens;

    public bool lockRotation = false;
    public bool singleHandControls = true;

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
            if (!singleHandControls)
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
            else
            {
                float keyValueX;
                float keyValueY;

                if (Input.GetKey(KeyCode.Y))
                {
                    keyValueY = 0.5f;
                }
                else if (Input.GetKey(KeyCode.H))
                {
                    keyValueY = -0.5f;
                }
                else
                {
                    keyValueY = 0f;
                }

                if (Input.GetKey(KeyCode.J))
                {
                    keyValueX = 0.5f;
                }
                else if (Input.GetKey(KeyCode.G))
                {
                    keyValueX = -0.5f;
                }
                else
                {
                    keyValueX = 0f;
                }
                float camX = keyValueX * Time.deltaTime * sens;
                float camY = keyValueY * Time.deltaTime * sens;
                yRotation += camX;
                xRotation -= camY;


                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                //rotate cam and orientaton
                transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            }
        }

       

    }
}
