using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ToolBoxCanvas : MonoBehaviour
{
    #region Public Params

    [Header("Settings")]
    public int TotalLocks = 3;
    [SerializeField] float RadiusOfLock;
    [HideInInspector] public float difficulty = 0;

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
    BoxCollider2D lockPickCollider, lockGoalCollider;

    #endregion

    #region Init and Update
    
    private void Start() {
        lockPickCollider = lockPick.GetComponent<BoxCollider2D>();
        lockGoalCollider = lockGoal.GetComponent<BoxCollider2D>();

        CreateStars();

        // create moving lock
        lockRotation = lockPick.GetComponent<AnchoredRotation>();
        lockRotation.SetRotationSpeed(difficulty);
        RadiusOfLock = lockPick.transform.localPosition.y;
        MoveLockGoal();

        // change cursor
        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.ForceSoftware);
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

            currentLocksBroken += 1;
            StartCoroutine(lockRotation.FreezeObject(0.3f));
            if (currentLocksBroken >= TotalLocks) {
                OpenToolBox.Invoke();
            } else {
                MoveLockGoal();
            }

        } else { // fail
            singleAudio.PlaySFX("Fail");

            StartCoroutine(lockRotation.ShakeObject(0.75f)); // stop spinning dot
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
