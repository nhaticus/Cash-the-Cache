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
    [SerializeField] GameObject LockGoalPrefab;
    [SerializeField] Transform LockGoalParent;
    [SerializeField] TMP_Text locksLeft;

    [HideInInspector] public UnityEvent OpenToolBox;

    #endregion

    #region Private Params

    GameObject currentLockGoal;
    int currentLocksBroken = 0;

    #endregion

    #region Init and Update
    
    private void Start() {
        lockRotation = LockPick.GetComponent<AnchoredRotation>();
        lockRotation.SetRotationSpeed(difficulty);
        RadiusOfLock = LockPick.transform.localPosition.y;
        locksLeft.text = "Locks left: " + TotalLocks.ToString();
        SpawnLockGoal();
    }

    private void Update() {
        if ((UserInput.Instance && (UserInput.Instance.Cancel || UserInput.Instance.Pause)) || (!UserInput.Instance && Input.GetKeyDown(KeyCode.Escape))) {
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
        if (LockPick.GetComponent<BoxCollider2D>().IsTouching(currentLockGoal.GetComponent<BoxCollider2D>())) {

            currentLocksBroken += 1;
            locksLeft.text = "Locks left: " + (TotalLocks - currentLocksBroken).ToString();
            if (currentLocksBroken >= TotalLocks) {
                OpenToolBox.Invoke();
            } else {
                SpawnLockGoal();
            }

        } else {
            // stop spinning dot
            StartCoroutine(lockRotation.ShakeObject());
        }
    }

    #endregion

    #region Private Functions
    private void SpawnLockGoal() {
        if (currentLockGoal != null) {
            Destroy(currentLockGoal);
        }
        var pos = Random.insideUnitCircle.normalized * RadiusOfLock;
        currentLockGoal = Instantiate(LockGoalPrefab, (Vector2)LockGoalParent.position - pos, Quaternion.identity, LockGoalParent);
    }

    #endregion
}
