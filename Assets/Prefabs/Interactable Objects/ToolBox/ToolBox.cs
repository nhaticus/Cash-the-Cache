using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBox : MonoBehaviour, InteractEvent {

    [SerializeField] GameObject[] obj;
    [SerializeField] int difficulty = 4;

    [SerializeField] GameObject toolboxCanvas;

    public void Interact() {
        if(AnalyticsManager.Instance)
            AnalyticsManager.Instance.TrackMinigameStarted("Toolbox Minigame");

        GameObject canvas = Instantiate(toolboxCanvas, transform);
        canvas.GetComponent<ToolBoxCanvas>().OpenToolBox.AddListener(OpenToolBox);
        canvas.GetComponent<ToolBoxCanvas>().difficulty = difficulty;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);
    }

    void OpenToolBox() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        // spawn a random object at box position
        if(obj.Length > 0)
            Instantiate(obj[Random.Range(0, obj.Length - 1)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
