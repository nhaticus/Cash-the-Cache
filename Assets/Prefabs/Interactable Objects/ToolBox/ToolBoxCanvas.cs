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
    [SerializeField] GameObject LockPick;
    AnchoredRotation lockRotation;
    [SerializeField] GameObject lockGoal;
    [SerializeField] Transform LockGoalParent;

    [Header("Animation")]
    [SerializeField] Transform starGridTransform;
    [SerializeField] GameObject starObj;
    [SerializeField] float starSpacing = 35;
    List<GameObject> starList = new List<GameObject>();

    [HideInInspector] public UnityEvent OpenToolBox;

    #endregion

    #region Private Params

    int currentLocksBroken = 0;
    BoxCollider2D lockPickCollider, lockGoalCollider;

    #endregion

    #region Init and Update
    
    private void Start() {
        lockPickCollider = LockPick.GetComponent<BoxCollider2D>();
        lockGoalCollider = lockGoal.GetComponent<BoxCollider2D>();

        CreateStars();

        lockRotation = LockPick.GetComponent<AnchoredRotation>();
        lockRotation.SetRotationSpeed(difficulty);
        RadiusOfLock = LockPick.transform.localPosition.y;
        MoveLockGoal();
    }

    private void Update() {
        if ((UserInput.Instance && (UserInput.Instance.Cancel || UserInput.Instance.Pause)) ||
            (!UserInput.Instance && Input.GetKeyDown(KeyCode.Escape))) {
            ExitToolBox();
        }
    }

    #endregion

    #region Public Functions

    public void ExitToolBox() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();
        Destroy(gameObject);
    }

    public void TryPick() {
        if (lockPickCollider.IsTouching(lockGoalCollider)) {
            starList[currentLocksBroken].GetComponent<Animator>().SetBool("Full", true);
            currentLocksBroken += 1;
            StartCoroutine(lockRotation.FreezeObject(0.3f));
            if (currentLocksBroken >= TotalLocks) {
                OpenToolBox.Invoke();
            } else {
                MoveLockGoal();
            }

        } else {
            // stop spinning dot
            StartCoroutine(lockRotation.ShakeObject(0.75f));
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
