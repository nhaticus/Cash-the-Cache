// Flushing canvas handles the flushing minigame, as well as the creation of the canvas
// Player has to sync up the target and the controlled box in order to raise the meter, and doing it enough flushes the toilet

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FlushingCanvas : MonoBehaviour
{
    [Header("Flushing Settings")]
    public int difficulty;

    [SerializeField] Slider powerSlider;
    [HideInInspector] public UnityEvent toiletOpened;
    [SerializeField] GameObject targetObject;
    [SerializeField] GameObject controlledObject;
    [SerializeField] float speed;
    [SerializeField] GameObject button;
    [SerializeField] float lowerBound;
    [SerializeField] float upperBound;
    private void Start()
    {
        powerSlider.value = 0;
    }

    private void Update()
    {
        ApplyForceToControl((-0.5f)*speed);
        if(powerSlider.value >= 1)
        {
            toiletOpened.Invoke();
            ExitToilet();
        }
        if ((controlledObject.transform.localPosition.x < lowerBound && button.GetComponent<FlushButton>().SENDIT == false) || 
            (button.GetComponent<FlushButton>().SENDIT && controlledObject.transform.localPosition.x > upperBound))
        {
            controlledObject.GetComponent<Rigidbody2D>().drag = 9999999999999999999;
        }
        else
        {
            controlledObject.GetComponent<Rigidbody2D>().drag = 1;
        }
    }

    public void ExitToilet()
    {
        powerSlider.value = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        gameObject.SetActive(false);
    }

    private void ApplyForceToControl(float force)
    {
        controlledObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * Time.deltaTime, 0), ForceMode2D.Force);
    }

    public void FlushButton()
    {
        controlledObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(speed * Time.deltaTime, 0), ForceMode2D.Force);
    }
}