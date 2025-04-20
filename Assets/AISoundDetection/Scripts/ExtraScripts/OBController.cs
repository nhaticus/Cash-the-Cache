using UnityEngine;
//using UnityEngine.InputSystem;
namespace AiSoundDetect.Extra
{
public class OBController : MonoBehaviour
{
     //for using New InputSystem comment the part bellow until the Here IS NEW INPUT SYSTEM    with   start /*    end */
     
    int horizontalInput;
    int verticalInput;
    int wheelInput;
    [SerializeField]
    private float moveSpeed = 15.0f;
    private Vector3 moveVector;

    void Start()
    {
       moveVector = new Vector3(0, 0, 0);
    }

    void Update()
    {
       moveVector.x = Input.GetAxisRaw("Horizontal");
       moveVector.z = Input.GetAxisRaw("Vertical");

       if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) 
        {
          transform.position += moveSpeed * moveVector * Time.deltaTime;
        }
       
    }

       //          Here IS NEW INPUT SYSTEM 

    /*

  [SerializeField]private Rigidbody rb;
	[SerializeField]private float moveSpeed = 5f;
	
	public DefaultInputActions playerControls;
	Vector3 moveDirection = Vector3.zero;
	private InputAction move;
	
	
	private void Awake()
	{
		playerControls = new DefaultInputActions();
		rb = GetComponent<Rigidbody>();
	}
	private void OnEnable()
	{
		rb.isKinematic = false;
		move = playerControls.Player.Move;
		move.Enable();
		
	}
	private void OnDisable()
	{
		move.Disable();
	}
	
	void Update()
	{
		moveDirection = move.ReadValue<Vector2>();
	}
	private void FixedUpdate()
	{
		rb.velocity =new Vector3(moveDirection.x * moveSpeed, 0.0f,moveDirection.y * moveSpeed);
	}
	
	  */     
}
}