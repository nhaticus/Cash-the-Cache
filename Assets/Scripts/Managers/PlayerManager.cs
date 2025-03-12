using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    PlayerCam playerCameraScript;
    PlayerMovement playerMovementScript;

    List<Renderer> visualRenderers = new List<Renderer>(); //For changing the color of van outline when player has something

    //Player Stats
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity;

    [Header("Player Weight")]
    [SerializeField]
    private int weight = 0;
    private static int maxWeightDefault = 30;
    [SerializeField]
    private int maxWeight;

    [Header("Player Speed")]

    [SerializeField]
    private float slowdownAmount = 0.2f; // 0.2 = 80% slower
    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float maxSpeed;

    private static float moveSpeedDefault = 5f;

    public bool ableToInteract = true;

    //Create a Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //Finds reference to playerCamera 
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

        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void Start()
    {
        foreach (Items item in UpgradeManager.Instance.loadedItems)
        {
            if (item.itemName == "Running Shoes")
            {
                maxSpeed = moveSpeedDefault + (item.level * item.statValue);
                currentSpeed = maxSpeed;
            }
        }
        if (playerMovementScript != null)
        {
            playerMovementScript.moveSpeed = maxSpeed;
        }
        ableToInteract = true;

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 120);
        if (playerCameraScript)
        {
            playerCameraScript.sens = mouseSensitivity;
        }

        maxWeight = PlayerPrefs.GetInt("MaxWeight", maxWeightDefault);

    }


    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            playerCameraScript = mainCamera.GetComponent<PlayerCam>();
            if (playerCameraScript)
            {
                playerCameraScript.sens = mouseSensitivity;
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

        // Find all renderers in the visual area
        visualRenderers.Clear();
        GameObject visualArea = GameObject.Find("Visual area");
        if (visualArea != null)
        {
            // Get all child renderers
            Renderer[] renderers = visualArea.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                visualRenderers.Add(rend);
            }
        }
    }

    public void increaseMoveSpeed(float speedIncrease)
    {
        currentSpeed += speedIncrease;
        maxSpeed += speedIncrease;
        // Debug.Log("increasing Move speed");
    }

    public void decreaseMoveSpeed(float speedDecrease)
    {
        currentSpeed -= speedDecrease;
        maxSpeed -= speedDecrease;
        // Debug.Log("decreasing Move speed");
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
        UpdateVisualAreaColor(); // Update color when weight changes
    }

    public void subWeight(int itemWeight)
    {
        weight -= itemWeight;
        UpdateVisualAreaColor(); // Update color when weight changes
    }

    public void setWeight(int newWeight)
    {
        weight = newWeight;
        UpdateVisualAreaColor();
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

    private void UpdateVisualAreaColor()
    {
        Color newColor = (weight > 0) ? Color.green : Color.red;

        foreach (Renderer rend in visualRenderers)
        {
            rend.material.color = newColor;
        }
    }

    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        if (playerCameraScript)
            playerCameraScript.sens = mouseSensitivity;
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
            newSpeed = ChangeSpeedByPercent(10); // 10% slower
        else
            newSpeed = ChangeSpeedByPercent(0); //Player Inventory is empty

        Debug.Log("Player Speed set to: " + newSpeed.ToString());
        setMoveSpeed(newSpeed);
    }

    public void ResetDefault()
    {
        maxWeight = maxWeightDefault;
        PlayerPrefs.SetInt("MaxWeight", maxWeight);
    }
}
