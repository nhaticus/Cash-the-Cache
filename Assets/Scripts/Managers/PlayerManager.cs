using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    PlayerCam playerCameraScript;
    PlayerMovement playerMovementScript;

    //Player Stats
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity;
    
    [Header("Controller Sensitivity")]
    public float controllerSensitivity;

    [Header("Player Weight")]
    [SerializeField] int weight = 0;
    static int maxWeightDefault = 30;
    [SerializeField] int maxWeight;

    [Header("Player Speed")]
    [SerializeField] float slowdownAmount = 0.2f; // 0.2 = 80% slower
    [SerializeField] float currentSpeed;
    [SerializeField] float maxSpeed;

    private static float moveSpeedDefault = 5f;

    // STATS
    public bool hasFlashlight = false;
    float boxOpening = 1;

    public bool ableToInteract = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            playerCameraScript = mainCamera.GetComponent<PlayerCam>();
        }

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerMovementScript = player.GetComponent<PlayerMovement>();
        }

        LoadUpgrades();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            playerCameraScript = mainCamera.GetComponent<PlayerCam>();
            if (playerCameraScript)
            {
                playerCameraScript.mouseSens = mouseSensitivity;
                playerCameraScript.controllerSens = controllerSensitivity;
            }
        }

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerMovementScript = player.GetComponent<PlayerMovement>();
            if (playerMovementScript)
            {
                playerMovementScript.moveSpeed = maxSpeed;
                ableToInteract = true;
                unlockRotation();
            }
        }
        weight = 0;
        LoadUpgrades();
    }

    public void increaseMoveSpeed(float speedIncrease)
    {
        currentSpeed += speedIncrease;
        maxSpeed += speedIncrease;
    }

    public void increaseMaxMoveSpeed(float speedIncrease)
    {
        maxSpeed += speedIncrease;
    }

    public void decreaseMoveSpeed(float speedDecrease)
    {
        currentSpeed -= speedDecrease;
        maxSpeed -= speedDecrease;
    }

    public float getMoveSpeed()
    {
        return currentSpeed;
    }

    public void setMoveSpeed(float newSpeed)
    {
        currentSpeed = newSpeed;
        playerMovementScript.moveSpeed = newSpeed;
    }

    public float getMaxMoveSpeed()
    {
        return maxSpeed;
    }

    public void slowPlayer()
    {
        currentSpeed = maxSpeed * slowdownAmount;
        if (currentSpeed < 0)
            currentSpeed = 0;
        playerMovementScript.moveSpeed = currentSpeed;
    }

    public void unSlowPlayer()
    {
        currentSpeed = maxSpeed;
        playerMovementScript.moveSpeed = currentSpeed;
    }
    public float getSlowAmt()
    {
        return slowdownAmount;
    }

    public void addWeight(int itemWeight)
    {
        weight += itemWeight;
    }

    public void subWeight(int itemWeight)
    {
        weight -= itemWeight;
    }

    public void setWeight(int newWeight)
    {
        weight = newWeight;
    }

    public int getWeight()
    {
        return weight;
    }

    public void increaseMaxWeight(int increase)
    {
        maxWeight += increase;
        PlayerPrefs.SetInt("MaxWeight", maxWeight);
    }
    public void decreaseMaxWeight(int decrease)
    {
        maxWeight -= decrease;
        PlayerPrefs.SetInt("MaxWeight", maxWeight);
    }

    public int getMaxWeight()
    {
        return maxWeight;
    }

    public void IncreaseBoxOpening(float increase)
    {
        boxOpening += increase;
        PlayerPrefs.SetFloat("BoxOpening", boxOpening);
    }

    public float GetBoxOpening()
    {
        return boxOpening;
    }


    //Locks rotation of player camera
    public void lockRotation()
    {
        playerCameraScript.lockRotation = true;
    }

    //Unlocks rotation of player camera
    public void unlockRotation()
    {
        playerCameraScript.lockRotation = false;
    }

    public void ToggleRotation()
    {
        playerCameraScript.lockRotation = !playerCameraScript.lockRotation;
    }

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        if (playerCameraScript)
            playerCameraScript.mouseSens = mouseSensitivity;
    }
    public void SetControllerSensitivity(float sensitivity)
    {
        controllerSensitivity = sensitivity;
        if (playerCameraScript)
            playerCameraScript.controllerSens = controllerSensitivity;
    }

    public void WeightChangeSpeed()
    {
        float ChangeSpeedByPercent(float percent)
        {
            return getMaxMoveSpeed() - (getMaxMoveSpeed() * percent / 100);
        }

        float weightPercentage = (float)getWeight() / getMaxWeight();
        float newSpeed = getMaxMoveSpeed();
        if (weightPercentage >= 0.9)
            newSpeed = ChangeSpeedByPercent(40); // 40% slower
        else if (weightPercentage > 0.8)
            newSpeed = ChangeSpeedByPercent(25); // 25% slower
        else if (weightPercentage > 0.6)
            newSpeed = ChangeSpeedByPercent(12); // 12% slower
        else
            newSpeed = ChangeSpeedByPercent(0); //Player Inventory is empty

        Debug.Log("Player Speed set to: " + newSpeed.ToString());
        setMoveSpeed(newSpeed);
    }

    public void LoadUpgrades() {
        ableToInteract = true;

        maxSpeed = moveSpeedDefault + PlayerPrefs.GetInt("RunningShoe") * 0.5f;
        currentSpeed = maxSpeed;
        maxWeight = maxWeightDefault + PlayerPrefs.GetInt("Backpack") * 5;

        hasFlashlight = PlayerPrefs.GetInt("Flashlight") == 1;
        boxOpening = 1 + (PlayerPrefs.GetInt("Screwdriver", 0) * 0.3f);

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 120);
        controllerSensitivity = PlayerPrefs.GetFloat("Controller Sensitivity", 120);
        if (playerCameraScript)
        {
            playerCameraScript.mouseSens = mouseSensitivity;
            playerCameraScript.controllerSens = controllerSensitivity;
        }
    }

    public void ResetDefault()
    {
        maxWeight = maxWeightDefault;
        PlayerPrefs.SetInt("MaxWeight", maxWeight);
        maxSpeed = moveSpeedDefault;
        currentSpeed = maxSpeed;
        boxOpening = 1;
    }
}
