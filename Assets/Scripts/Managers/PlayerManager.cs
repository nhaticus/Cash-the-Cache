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
    private int maxWeight = 30;

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

    public void increaseMoveSpeed(float speedIncrease)
    {
        playerMovementScript.moveSpeed += speedIncrease;
    }

    public void decreaseMoveSpeed(float speedDecrease)
    {
        playerMovementScript.moveSpeed -= speedDecrease;
    }

    public float getMoveSpeed()
    {
        return playerMovementScript.moveSpeed;
    }

    public void addWeight(int itemWeight)
    {
        this.weight += itemWeight;
    }

    public void subWeight(int itemWeight)
    {
        this.weight -= itemWeight;
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
