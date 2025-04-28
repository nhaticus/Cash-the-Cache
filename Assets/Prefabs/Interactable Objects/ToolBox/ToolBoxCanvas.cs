using UnityEngine;
using UnityEngine.Events;

public class ToolBoxCanvas : MonoBehaviour
{
    #region Public Params

    [Header("Settings")]
    [SerializeField] int TotalLocks = 3;
    [SerializeField] float RadiusOfLock;
    [HideInInspector] public float difficulty = 0;

    [Header("Dependencies")]
    [SerializeField] GameObject LockPick;
    AnchoredRotation lockRotation;
    [SerializeField] GameObject LockGoalPrefab;
    [SerializeField] Transform LockGoalParent;

    [HideInInspector] public UnityEvent OpenToolBox;

    #endregion

    #region Private Params

    GameObject _currentLockGoal;
    int _currentLocksBroken = 0;

    #endregion

    #region Init and Update
    
    private void Start() {
        lockRotation = LockPick.GetComponent<AnchoredRotation>();
        lockRotation.SetRotationAmount(difficulty);
        RadiusOfLock = LockPick.transform.localPosition.y;
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
        if (LockPick.GetComponent<BoxCollider2D>().IsTouching(_currentLockGoal.GetComponent<BoxCollider2D>())) {

            _currentLocksBroken += 1;
            if (_currentLocksBroken >= TotalLocks) {
                OpenToolBox.Invoke();
            } else {
                SpawnLockGoal();
            }

        } else {
            Debug.Log("Pick Failed");
            // stop spinning dot
            StartCoroutine(lockRotation.ShakeObject());
        }
    }

    #endregion

    #region Private Functions
    private void SpawnLockGoal() {
        if (_currentLockGoal != null) {
            Destroy(_currentLockGoal);
        }
        var pos = Random.insideUnitCircle.normalized * RadiusOfLock;
        _currentLockGoal = Instantiate(LockGoalPrefab, (Vector2)LockGoalParent.position - pos, Quaternion.identity, LockGoalParent);
    }

    #endregion
}
