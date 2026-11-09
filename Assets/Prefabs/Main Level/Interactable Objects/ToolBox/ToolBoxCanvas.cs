using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolBoxCanvas : MonoBehaviour
{
    #region Public Params

    [Header("Settings")]
    public int TotalLocks = 3;
    public float failFreezeTime = 0.75f;
    [SerializeField] float RadiusOfLock;

    [Header("Difficulty")]
    [HideInInspector] public float difficulty = 0;
    [HideInInspector] public float strength;
    [SerializeField] float sizeMult = 0.12f; // increase target size based on strength
    [SerializeField] float failMult = 0.06f; // decrease fail time based on strength
    [SerializeField] float minFreezeTime = 0.5f;

    [Header("Dependencies")]
    [SerializeField] GameObject lockPick;
    AnchoredRotation lockRotation;
    [SerializeField] GameObject lockGoal;
    [SerializeField] Transform LockGoalParent;
    [SerializeField] SingleAudio singleAudio;

    [Header("Animation")]
    [SerializeField] Transform starGridTransform;
    [SerializeField] GameObject starObj;
    [SerializeField] float starSpacing = 35;
    List<GameObject> starList = new List<GameObject>();

    [Header("Canvas")]
    [SerializeField] Texture2D cursorImage;

    [HideInInspector] public UnityEvent OpenToolBox;

    #endregion

    #region Private Params

    int currentLocksBroken = 0;
    CircleCollider2D lockPickCollider, lockGoalCollider;

    #endregion

    #region Init and Update
    
    private void Start() {
        lockPickCollider = lockPick.GetComponent<CircleCollider2D>();
        lockGoalCollider = lockGoal.GetComponent<CircleCollider2D>();

        // create moving lock
        lockRotation = lockPick.GetComponent<AnchoredRotation>();
        lockRotation.SetRotationSpeed(difficulty);
        RadiusOfLock = lockPick.transform.localPosition.y;
        MoveLockGoal();

        // change cursor
        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.ForceSoftware);

        TotalLocks += Mathf.FloorToInt(difficulty * 0.5f);

        // get strength and increase goal size
        Item screwdriver = DataSystem.GetItem("Screwdriver");
        strength = screwdriver.level;
        lockGoal.transform.localScale += new Vector3(sizeMult * strength, sizeMult * strength, 0);

        // decrease fail time
        failFreezeTime -= failMult * strength;
        failFreezeTime = Mathf.Max(failFreezeTime, minFreezeTime);

        CreateStars();
    }

    private void Update() {
        if ((UserInput.Instance && UserInput.Instance.Cancel) ||
            (!UserInput.Instance && Input.GetKeyDown(KeyCode.Escape))) {
            ExitToolBox();
        }
    }

    #endregion

    #region Public Functions

    public void ExitToolBox() {
        // reset cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject);
    }

    public void TryPick() {
        if (lockPickCollider.IsTouching(lockGoalCollider)) { // success
            singleAudio.PlaySFX("Success"); // sound effect
            starList[currentLocksBroken].GetComponent<Animator>().SetBool("Full", true); // fill in star

            currentLocksBroken++;
            StartCoroutine(lockRotation.FreezeObject(0.3f));
            if (currentLocksBroken >= TotalLocks) {
                OpenToolBox.Invoke();
            } else {
                MoveLockGoal();
            }

        } else { // fail
            singleAudio.PlaySFX("Fail");

            StartCoroutine(lockRotation.ShakeObject(failFreezeTime)); // stop spinning dot
        }
    }

    #endregion

    #region Private Functions
    private void CreateStars()
    {
        for(int i = 0; i < TotalLocks; i++)
        {
            GameObject star = Instantiate(starObj, starGridTransform);
            float starWidth = star.GetComponent<RectTransform>().rect.width / 2;
            star.transform.position += new Vector3((starWidth + starSpacing) * i, 0, 0);
            starList.Add(star);
        }
    }

    private void MoveLockGoal() {
        var pos = Random.insideUnitCircle.normalized * RadiusOfLock;
        lockGoal.transform.position = (Vector2)LockGoalParent.position - pos;
    }

    #endregion
}
