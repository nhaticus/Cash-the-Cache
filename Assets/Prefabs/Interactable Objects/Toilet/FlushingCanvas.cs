using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/* 
 * Toilet Minigame canvas
 * Player has to sync up the target and the controlled box in order to raise the meter, and doing it enough flushes the toilet
*/

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

    [Header("Animation")]
    [SerializeField] Image toilet;
    [SerializeField] Sprite toiletUp, toiletDown;

    [Header("Sounds")]
    [SerializeField] SingleAudio singleAudio;
    [SerializeField] string plungeSound;

    bool plungerDown = false;

    private void Start()
    {
        powerSlider.value = 0;
    }

    private void Update()
    {
        ApplyForceToControl((-0.5f) * speed);
        if(powerSlider.value >= 1)
        {
            toiletOpened.Invoke();
            ExitToilet();
        }

        if (plungerDown)
            ApplyForceToControl(speed);
    }

    public void ExitToilet()
    {
        powerSlider.value = 0;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        gameObject.SetActive(false);
    }

    private void ApplyForceToControl(float force)
    {
        controlledObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(force * Time.deltaTime, 0), ForceMode2D.Force);
    }

    public void PlungeButton()
    {
        toilet.sprite = toiletDown;
        singleAudio.PlaySFX(plungeSound);
        plungerDown = true;
    }

    public void StopPlunge()
    {
        toilet.sprite = toiletUp;
        plungerDown = false;
    }
}