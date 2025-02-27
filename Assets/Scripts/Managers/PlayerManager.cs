using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerCam playerCameraScript;
    private PlayerMovement playerMovementScript;

    private List<Renderer> visualRenderers = new List<Renderer>(); //For changing the color of van outline when player has something

    //Player Stats
    [SerializeField]
    private int weight = 0;

    [SerializeField]
    private int maxWeight = 30;

    [SerializeField]
    private float slowdownAmount = 9;

    [SerializeField]
    private float currentSpeed;
    [SerializeField]
    private float maxSpeed;

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
        playerCameraScript = GameObject.Find("Main Camera").GetComponent<PlayerCam>();
        playerMovementScript = GameObject.Find("Player").GetComponent<PlayerMovement>();

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
        }
    }
    private void Start()
    {
        this.currentSpeed = playerMovementScript.moveSpeed;
        this.maxSpeed = playerMovementScript.moveSpeed;
        this.ableToInteract = true;
    }

    public float getSlowAmt()
    {
        return this.slowdownAmount;
    }

    public void increaseMoveSpeed(float speedIncrease)
    {
        this.maxSpeed += speedIncrease;
        this.slowdownAmount += speedIncrease;
        Debug.Log("increasing Move speed");
    }

    public void decreaseMoveSpeed(float speedDecrease)
    {
        this.maxSpeed -= speedDecrease;
        this.slowdownAmount -= speedDecrease;
        Debug.Log("decreasing Move speed");
    }

    public float getMoveSpeed()
    {
        return this.currentSpeed;
    }

    public void setMoveSpeed(float newSpeed)
    {
        this.currentSpeed = newSpeed;
        playerMovementScript.moveSpeed = newSpeed;
    }

    public float getMaxMoveSpeed()
    {
        return this.maxSpeed;
    }

    public void slowPlayer()
    {
        this.currentSpeed -= this.slowdownAmount;
        playerMovementScript.moveSpeed -= this.slowdownAmount;
    }

    public void unSlowPlayer()
    {
        this.currentSpeed = this.maxSpeed;
        playerMovementScript.moveSpeed = maxSpeed;
    }

    public void addWeight(int itemWeight)
    {
        this.weight += itemWeight;
        UpdateVisualAreaColor(); // Update color when weight changes
    }

    public void subWeight(int itemWeight)
    {
        this.weight -= itemWeight;
        UpdateVisualAreaColor(); // Update color when weight changes
    }

    public void setWeight(int newWeight)
    {
        this.weight = newWeight;
        UpdateVisualAreaColor();
    }

    public int getWeight()
    {
        return this.weight;
    }

    public void increaseMaxWeight(int increase)
    {
        this.maxWeight += increase;
    }
    public void decreaseMaxWeight(int decrease)
    {
        this.maxWeight -= decrease;
    }

    public int getMaxWeight()
    {
        return this.maxWeight;
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
}
