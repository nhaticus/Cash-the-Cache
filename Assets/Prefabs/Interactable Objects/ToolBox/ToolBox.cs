using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBox : MonoBehaviour, InteractEvent {

    [SerializeField] GameObject[] loot;

    [Header("Difficulty")]
    [SerializeField] int difficulty = 4;
    [SerializeField] bool setRandomDifficulty;
    [SerializeField] int minDifficulty = 3, maxDifficulty = 7;

    [Header("Canvas")]
    [SerializeField] GameObject toolboxCanvas;
    [SerializeField] Texture2D cursorImage;


    private void Start()
    {
        if (setRandomDifficulty)
            difficulty = Random.Range(minDifficulty, maxDifficulty);

        difficulty += PlayerPrefs.GetInt("Difficulty") / 3;
        if (difficulty > maxDifficulty)
            difficulty = maxDifficulty;
    }

    public void Interact() {
        //if(AnalyticsManager.Instance)
        //    AnalyticsManager.Instance.TrackMinigameStarted("Toolbox Minigame");

        // create toolbox canvas
        GameObject canvas = Instantiate(toolboxCanvas, transform);
        canvas.GetComponent<ToolBoxCanvas>().OpenToolBox.AddListener(OpenToolBox);
        canvas.GetComponent<ToolBoxCanvas>().difficulty = difficulty;

        // set cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.ForceSoftware);

        PlayerManager.Instance.ableToInteract = false;
        PlayerManager.Instance.lockRotation();
        PlayerManager.Instance.setMoveSpeed(0);
    }

    void OpenToolBox() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);

        PlayerManager.Instance.ableToInteract = true;
        PlayerManager.Instance.unlockRotation();
        PlayerManager.Instance.WeightChangeSpeed();

        SpawnLoot();

        Destroy(gameObject);
    }

    private void SpawnLoot()
    {
        int spawnAmount = 0;

        if (difficulty <= 3)
            spawnAmount = 1;
        else if (difficulty <= 5)
            spawnAmount = 2;
        else
            spawnAmount = 4;

        for (int i = 0; i < spawnAmount; i++)
        {
            Instantiate(loot[Random.Range(0, loot.Length)], transform.position, transform.rotation);
        }
    }
}
