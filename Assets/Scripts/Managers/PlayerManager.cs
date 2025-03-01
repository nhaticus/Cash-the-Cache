using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    PlayerCam playerCameraScript;
    PlayerMovement playerMovementScript;

    List<Renderer> visualRenderers = new List<Renderer>(); //For changing the color of van outline when player has something

    //Player Stats
    public float mouseSensitivity;
    [SerializeField]
    int weight = 0;

    [SerializeField]
    int maxWeight = 30;

    [SerializeField]
    float slowdownAmount;
    [SerializeField]
    float currentSpeed;
    [SerializeField]
    float maxSpeed;

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

        // Find all renderers in the visual area
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

        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            playerCameraScript = mainCamera.GetComponent<PlayerCam>();
        }

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerMovementScript = player.GetComponent<PlayerMovement>();
            ableToInteract = true;
            unlockRotation();
        }
    }
    private void Start()
    {
        if (playerMovementScript != null)
        {
            currentSpeed = playerMovementScript.moveSpeed;
            maxSpeed = playerMovementScript.moveSpeed;
            slowdownAmount = maxSpeed * 0.8f;
        }
        ableToInteract = true;

        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 120);
        if (playerCameraScript)
        {
            Debug.Log(mouseSensitivity);
            playerCameraScript.sens = mouseSensitivity;
        }
    }

    

    public void increaseMoveSpeed(float speedIncrease)
    {
        maxSpeed += speedIncrease;
        slowdownAmount += speedIncrease;
        Debug.Log("increasing Move speed");
    }

    public void decreaseMoveSpeed(float speedDecrease)
    {
        maxSpeed -= speedDecrease;
        slowdownAmount -= speedDecrease;
        Debug.Log("decreasing Move speed");
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
        currentSpeed -= slowdownAmount;
        if(currentSpeed < 0)
            playerMovementScript.moveSpeed = 0;
        else
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
    }
    public void decreaseMaxWeight(int decrease)
    {
        maxWeight -= decrease;
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

    public void SetSensitivity(int sensitivity)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        mouseSensitivity = sensitivity;
        if (playerCameraScript)
            playerCameraScript.sens = mouseSensitivity;
    }
}
