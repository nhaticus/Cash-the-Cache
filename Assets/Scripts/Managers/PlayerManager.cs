using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerCam playerCameraScript;
    private PlayerMovement playerMovementScript;

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

    public bool ableToInteract = false;

    //Create a Singleton
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Finds reference to playerCamera 
        playerCameraScript = GameObject.Find("Main Camera").GetComponent<PlayerCam>();
        playerMovementScript = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        this.currentSpeed = playerMovementScript.moveSpeed;
        this.maxSpeed = playerMovementScript.moveSpeed;
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
    }

    public void subWeight(int itemWeight)
    {
        this.weight -= itemWeight;
    }

    public void setWeight(int newWeight)
    {
        this.weight = newWeight;
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
}
